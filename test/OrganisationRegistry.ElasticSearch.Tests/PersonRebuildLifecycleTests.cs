namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Threading.Tasks;
using Body.Events;
using Client;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using People;
using Person.Events;
using Projections.Infrastructure;
using Projections.People;
using OrganisationRegistry.Tests.Shared;
using Scenario;
using SqlServer.ElasticSearchProjections;
using SqlServer.Infrastructure;
using Xunit;

/// <summary>
/// Walks the full rebuild loop against a real index: project a person, delete the document behind the
/// projection's back, let a later event run into the resulting 404, and check that the person is queued
/// and actually restored by the individual rebuild runner.
/// </summary>
[Collection(nameof(ElasticSearchFixture))]
public class PersonRebuildLifecycleTests
{
    private readonly ElasticSearchFixture _fixture;

    public PersonRebuildLifecycleTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task DeletedPersonDocument_IsQueuedOnTheNextEventAndRebuilt()
    {
        var personId = Guid.NewGuid();
        var scenario = new PersonScenario(personId);
        var personCreated = scenario.Create<PersonCreated>();
        var assignedToBodySeat = scenario.Create<AssignedPersonToBodySeat>();

        var eventStore = new InMemoryEventStore();
        var projectionStates = new InMemoryProjectionStates();

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;
        var contextFactory = new TestContextFactory(dbContextOptions);

        // The mandate handler reads the seat from the projection cache once it gets to the document.
        await using (var seedContext = contextFactory.Create())
        {
            seedContext.BodySeatCache.Add(
                new BodySeatCacheItem
                {
                    Id = assignedToBodySeat.BodySeatId,
                    Name = assignedToBodySeat.BodySeatName,
                    Number = assignedToBodySeat.BodySeatNumber,
                    IsPaid = false,
                });
            await seedContext.SaveChangesAsync();
        }

        var peopleRunner = CreatePeopleRunner(eventStore, projectionStates, contextFactory);
        var rebuildRunner = CreateRebuildRunner(eventStore, projectionStates, contextFactory);

        // 1. The person is created and projected. First run, so the runner initialises the index itself.
        eventStore.Append(personCreated);
        await peopleRunner.Run();

        ReadPerson(personId).Source.Should().NotBeNull();
        ReadPerson(personId).Source.Name.Should().Be(personCreated.Name);

        // 2. The document disappears from ElasticSearch without the projection knowing.
        var delete = await _fixture.Elastic.WriteClient.DeleteAsync<PersonDocument>(personId);
        delete.IsValid.Should().BeTrue(delete.DebugInformation);
        ReadPerson(personId).Source.Should().BeNull();

        // 3. A body event touches that person again. The lookup 404s, so the projection stalls and the
        //    person -- not the body -- has to be queued for rebuild.
        eventStore.Append(assignedToBodySeat);

        await Assert.ThrowsAsync<ElasticsearchAggregateNotFoundException<PersonDocument>>(peopleRunner.Run);

        await using (var context = contextFactory.Create())
        {
            (await context.PeopleToRebuild.ToListAsync())
                .Should().ContainSingle().Which.PersonId.Should().Be(personId);
            (await context.BodiesToRebuild.ToListAsync()).Should().BeEmpty();
            (await context.OrganisationsToRebuild.ToListAsync()).Should().BeEmpty();
        }

        // 4. The individual rebuild runner replays the person's own events and puts the document back.
        await rebuildRunner.Run();

        ReadPerson(personId).Source.Should().NotBeNull();
        ReadPerson(personId).Source.Name.Should().Be(personCreated.Name);

        await using (var context = contextFactory.Create())
        {
            (await context.PeopleToRebuild.ToListAsync()).Should().BeEmpty();
        }

        // 5. And the projection can move on: the body event now finds its document.
        await peopleRunner.Run();

        ReadPerson(personId).Source.Mandates.Should().ContainSingle();
    }

    [Fact]
    public async Task QueuedPersonWithoutEnvelopes_IsDroppedInsteadOfStallingTheRunner()
    {
        var personId = Guid.NewGuid();

        var eventStore = new InMemoryEventStore();
        var projectionStates = new InMemoryProjectionStates();

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;
        var contextFactory = new TestContextFactory(dbContextOptions);

        // The person is queued but the projection has not advanced far enough to see any of its events,
        // so a replay has nothing to work with. That happens when the aggregate is created later in the
        // same batch that fails: the projection state is still behind the creating envelope.
        await using (var seedContext = contextFactory.Create())
        {
            seedContext.PeopleToRebuild.Add(new PersonToRebuild { PersonId = personId });
            await seedContext.SaveChangesAsync();
        }

        var rebuildRunner = CreateRebuildRunner(eventStore, projectionStates, contextFactory);

        await rebuildRunner.Run();

        await using (var context = contextFactory.Create())
        {
            (await context.PeopleToRebuild.ToListAsync()).Should().BeEmpty();
        }
    }

    private Osc.IGetResponse<PersonDocument> ReadPerson(Guid personId)
        => _fixture.Elastic.ReadClient.Get<PersonDocument>(personId);

    private PeopleRunner CreatePeopleRunner(
        InMemoryEventStore eventStore,
        InMemoryProjectionStates projectionStates,
        TestContextFactory contextFactory)
    {
        var (bus, busRegistrar) = CreateBus(contextFactory, eventStore);

        return new PeopleRunner(
            new NullLogger<PeopleRunner>(),
            _fixture.ElasticSearchOptions,
            eventStore,
            projectionStates,
            _fixture.Elastic,
            bus,
            busRegistrar,
            contextFactory);
    }

    private IndividualPersonRebuildRunner CreateRebuildRunner(
        InMemoryEventStore eventStore,
        InMemoryProjectionStates projectionStates,
        TestContextFactory contextFactory)
    {
        var (bus, busRegistrar) = CreateBus(contextFactory, eventStore);

        return new IndividualPersonRebuildRunner(
            new NullLogger<IndividualPersonRebuildRunner>(),
            eventStore,
            contextFactory,
            projectionStates,
            bus,
            _fixture.Elastic,
            busRegistrar);
    }

    private (ElasticBus Bus, ElasticBusRegistrar BusRegistrar) CreateBus(
        TestContextFactory contextFactory,
        InMemoryEventStore eventStore)
    {
        var serviceProvider = ProjectionHandlerServiceProvider.Build(
            contextFactory, _fixture, eventStore, PeopleRunner.EventHandlers);

        var bus = new ElasticBus(new NullLogger<ElasticBus>());

        return (bus, new ElasticBusRegistrar(new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider));
    }
}
