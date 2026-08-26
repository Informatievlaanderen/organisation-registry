namespace OrganisationRegistry.ElasticSearch.Tests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bodies;
using Body.Events;
using Client;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Organisation.Events;
using Organisations;
using People;
using Projections.Body;
using Projections.Infrastructure;
using Projections.Organisations;
using Projections.People;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Tests.Shared;
using Scenario;
using SqlServer;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using Xunit;

/// <summary>
/// When a runner cannot find a document in ElasticSearch, the aggregate has to be queued in the
/// rebuild table belonging to the <em>missing document's</em> type — not to the runner it happened in.
/// A person that 404s while body events are being projected must come back as a person to rebuild.
/// </summary>
[Collection(nameof(ElasticSearchFixture))]
public class RebuildQueueDispatchTests
{
    private const int LastProcessedEventNumber = 1;

    private readonly ElasticSearchFixture _fixture;

    public RebuildQueueDispatchTests(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task MissingPersonDocument_QueuesThePersonToRebuild()
    {
        var personId = Guid.NewGuid();
        var scenario = new PersonScenario(personId);

        // A body event that updates the person document: the runner has to look the person up in ES.
        var runner = BuildRunner(
            PeopleRunner.EventHandlers,
            scenario.Create<InitialiseProjection>(),
            scenario.Create<AssignedPersonToBodySeat>(),
            out var contextFactory,
            (store, projectionStates, bus, busRegistrar, contexts) => new PeopleRunner(
                new NullLogger<PeopleRunner>(), _fixture.ElasticSearchOptions, store, projectionStates,
                _fixture.Elastic, bus, busRegistrar, contexts).Run);

        await Assert.ThrowsAsync<ElasticsearchAggregateNotFoundException<PersonDocument>>(runner);

        await using var context = contextFactory.Create();
        (await context.PeopleToRebuild.ToListAsync())
            .Should().ContainSingle().Which.PersonId.Should().Be(personId);
        (await context.OrganisationsToRebuild.ToListAsync()).Should().BeEmpty();
        (await context.BodiesToRebuild.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task MissingBodyDocument_QueuesTheBodyToRebuild()
    {
        var bodyId = Guid.NewGuid();
        var scenario = new BodyScenario(bodyId);

        var runner = BuildRunner(
            BodyRunner.EventHandlers,
            scenario.Create<InitialiseProjection>(),
            scenario.Create<BodyInfoChanged>(),
            out var contextFactory,
            (store, projectionStates, bus, busRegistrar, contexts) => new BodyRunner(
                new NullLogger<BodyRunner>(), _fixture.ElasticSearchOptions, store, projectionStates,
                _fixture.Elastic, bus, busRegistrar, contexts).Run);

        await Assert.ThrowsAsync<ElasticsearchAggregateNotFoundException<BodyDocument>>(runner);

        await using var context = contextFactory.Create();
        (await context.BodiesToRebuild.ToListAsync())
            .Should().ContainSingle().Which.BodyId.Should().Be(bodyId);
        (await context.OrganisationsToRebuild.ToListAsync()).Should().BeEmpty();
        (await context.PeopleToRebuild.ToListAsync()).Should().BeEmpty();
    }

    [Fact]
    public async Task MissingOrganisationDocument_QueuesTheOrganisationToRebuild()
    {
        var organisationId = Guid.NewGuid();
        var scenario = new OrganisationScenario(organisationId);

        var runner = BuildRunner(
            OrganisationsRunner.EventHandlers,
            scenario.Create<InitialiseProjection>(),
            scenario.Create<OrganisationNameUpdated>(),
            out var contextFactory,
            (store, projectionStates, bus, busRegistrar, contexts) => new OrganisationsRunner(
                new NullLogger<OrganisationsRunner>(), _fixture.ElasticSearchOptions, store, projectionStates,
                _fixture.Elastic, bus, busRegistrar, contexts).Run);

        await Assert.ThrowsAsync<ElasticsearchAggregateNotFoundException<OrganisationDocument>>(runner);

        await using var context = contextFactory.Create();
        (await context.OrganisationsToRebuild.ToListAsync())
            .Should().ContainSingle().Which.OrganisationId.Should().Be(organisationId);
        (await context.BodiesToRebuild.ToListAsync()).Should().BeEmpty();
        (await context.PeopleToRebuild.ToListAsync()).Should().BeEmpty();
    }

    private Func<Task> BuildRunner<TInitialise, TEvent>(
        Type[] eventHandlers,
        TInitialise initialiseProjection,
        TEvent eventToProject,
        out TestContextFactory contextFactory,
        Func<IEventStore, IProjectionStates, ElasticBus, ElasticBusRegistrar, IContextFactory, Func<Task>> createRunner)
        where TInitialise : IEvent
        where TEvent : IEvent
    {
        var envelopes = new List<IEnvelope>
        {
            initialiseProjection.ToEnvelope(
                LastProcessedEventNumber + 1, string.Empty, string.Empty, string.Empty, string.Empty),
            eventToProject.ToEnvelope(
                LastProcessedEventNumber + 2, string.Empty, string.Empty, string.Empty, string.Empty),
        };

        var eventStore = new Mock<IEventStore>();
        eventStore.Setup(x => x.GetLastEvent()).Returns(LastProcessedEventNumber + envelopes.Count);
        eventStore
            .Setup(x => x.GetEventEnvelopesAfter(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Type[]>()))
            .Returns(envelopes);

        var projectionStates = new Mock<IProjectionStates>();
        // Anything but -1, so the runner does not initialise the projection itself.
        projectionStates
            .Setup(x => x.GetLastProcessedEventNumber(It.IsAny<string>()))
            .ReturnsAsync(LastProcessedEventNumber);

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;
        contextFactory = new TestContextFactory(dbContextOptions);

        var serviceProvider = ProjectionHandlerServiceProvider.Build(contextFactory, _fixture, eventHandlers);
        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        var busRegistrar = new ElasticBusRegistrar(
            new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);

        return createRunner(
            eventStore.Object, projectionStates.Object, bus, busRegistrar, contextFactory);
    }
}
