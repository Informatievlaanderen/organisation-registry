namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Threading.Tasks;
using Body.Events;
using Client;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Organisation.Events;
using People;
using Person;
using Person.Events;
using OrganisationRegistry.Infrastructure.Events;
using Projections.Infrastructure;
using Projections.People;
using OrganisationRegistry.Tests.Shared;
using Scenario;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using Xunit;

/// <summary>
/// A mass change flushes the document cache mid-batch, and that cache spans the whole batch. Both tests
/// cover what that means in practice: the envelope a flush blows up on is not the envelope that caused
/// it, and a mass change that succeeded has to leave the projection state behind it.
/// </summary>
[Collection(nameof(ElasticSearchFixture))]
public class MassChangeFlushTests
{
    private readonly ElasticSearchFixture _fixture;

    public MassChangeFlushTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MassChangeFlushingABrokenDocumentFromAnEarlierEnvelope_NamesThatDocument()
    {
        var personId = Guid.NewGuid();
        var scenario = new PersonScenario(personId);

        var eventStore = new InMemoryEventStore();
        var projectionStates = new InMemoryProjectionStates();
        var contextFactory = CreateContextFactory();
        var runner = CreatePeopleRunner(eventStore, projectionStates, contextFactory);

        // Envelope #1 puts a person without a name in the cache. Nothing fails yet: the cache is only
        // checked when it is written out.
        var personWithoutAName = new PersonCreated(
            personId, scenario.Create<string>(), name: string.Empty, sex: null, dateOfBirth: null);
        eventStore.Append(personWithoutAName);

        // Envelope #2 contributes no document of its own -- in the people projection an organisation
        // rename is nothing but mass changes -- yet it is the one that forces the flush.
        var organisationRenamed = scenario.Create<OrganisationNameUpdated>();
        eventStore.Append(organisationRenamed);

        var exception = await Assert.ThrowsAsync<Exception>(runner.Run);

        exception.Message.Should().Contain("Found document without key or name");
        exception.Message.Should().Contain(personId.ToString());
        exception.Message.Should().Contain("empty name");
        // changeId points back at the envelope that actually produced the document.
        exception.Message.Should().Contain("changeId 1");
        exception.Message.Should().Contain(nameof(PersonDocument));
    }

    [Fact]
    public async Task MassChangeAsLastChangeOfAnEnvelope_CommitsTheProjectionState()
    {
        var scenario = new PersonScenario(Guid.NewGuid());

        var eventStore = new InMemoryEventStore();
        var projectionStates = new InMemoryProjectionStates();
        var contextFactory = CreateContextFactory();
        var runner = CreatePeopleRunner(eventStore, projectionStates, contextFactory);

        // Envelope #1 is nothing but mass changes, so the last change in its set is a mass change.
        eventStore.Append(scenario.Create<OrganisationNameUpdated>());

        // Envelope #2 fails, so the batch never reaches the flush and state update at the end of Run().
        // Whatever the projection state ends up being was committed by envelope #1.
        eventStore.Append(scenario.Create<AssignedPersonToBodySeat>());

        await Assert.ThrowsAsync<ElasticsearchAggregateNotFoundException<PersonDocument>>(runner.Run);

        var lastProcessedEventNumber =
            await projectionStates.GetLastProcessedEventNumber(PeopleRunner.ElasticSearchProjectionsProjectionName);

        lastProcessedEventNumber.Should().Be(1);
    }

    private static TestContextFactory CreateContextFactory()
        => new(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options);

    private PeopleRunner CreatePeopleRunner(
        IEventStore eventStore,
        IProjectionStates projectionStates,
        TestContextFactory contextFactory)
    {
        var serviceProvider = ProjectionHandlerServiceProvider.Build(
            contextFactory, _fixture, eventStore, PeopleRunner.EventHandlers);

        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        var busRegistrar = new ElasticBusRegistrar(
            new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);

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
}
