namespace OrganisationRegistry.ElasticSearch.Tests.IndividualRebuildRunners;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Projections;
using OrganisationRegistry.ElasticSearch.Client;
using OrganisationRegistry.ElasticSearch.Configuration;
using OrganisationRegistry.ElasticSearch.Projections.Infrastructure;
using OrganisationRegistry.ElasticSearch.Projections.People.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using Infrastructure.Events;
using SqlServer;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.ProjectionState;
using OrganisationRegistry.Tests.Shared;
using OrganisationRegistry.Tests.Shared.Stubs;
using Xunit;

/// <summary>
/// Base class for testing the three individual rebuild runners.
/// Concrete tests supply the event handlers to register, the runner to exercise,
/// and the per-type test scaffolding (seed, envelopes, verify).
/// </summary>
public abstract class IndividualRebuildRunnerTestBase<TAggregate>
{
    private readonly ElasticSearchFixture _fixture;

    protected IndividualRebuildRunnerTestBase(ElasticSearchFixture fixture)
    {
        _fixture = fixture;
    }

    protected abstract Type[] EventHandlers { get; }

    protected abstract Func<Task> CreateRunner(
        IEventStore eventStore,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar);

    protected abstract List<IEnvelope> CreateEnvelopes(Guid aggregateId);
    protected abstract void SeedToRebuild(OrganisationRegistryContext context, Guid aggregateId);
    protected abstract Task Verify(
        ElasticSearchFixture fixture, IContextFactory contextFactory, Guid aggregateId);

    [Fact]
    public async Task Run_RebuildsDocumentAndRemovesPendingRow()
    {
        var aggregateId = Guid.NewGuid();
        var envelopes = CreateEnvelopes(aggregateId);

        var eventStoreMock = new Mock<IEventStore>();
        eventStoreMock
            .Setup(x => x.GetEventEnvelopesUntil<TAggregate>(aggregateId, It.IsAny<int>()))
            .Returns(envelopes);

        var projectionStatesMock = new Mock<IProjectionStates>();
        projectionStatesMock
            .Setup(x => x.GetLastProcessedEventNumber(It.IsAny<string>()))
            .ReturnsAsync(int.MaxValue);

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase($"org-es-test-{Guid.NewGuid()}", _ => { }).Options;
        var contextFactory = new TestContextFactory(dbContextOptions);

        await using (var seedContext = contextFactory.Create())
        {
            SeedToRebuild(seedContext, aggregateId);
            await seedContext.SaveChangesAsync();
        }

        var serviceProvider = ProjectionHandlerServiceProvider.Build(contextFactory, _fixture, EventHandlers);
        var bus = new ElasticBus(new NullLogger<ElasticBus>());
        var busRegistrar = new ElasticBusRegistrar(
            new NullLogger<ElasticBusRegistrar>(), bus, () => serviceProvider);

        var run = CreateRunner(
            eventStoreMock.Object, contextFactory, projectionStatesMock.Object, bus, _fixture.Elastic, busRegistrar);

        await run();

        await Verify(_fixture, contextFactory, aggregateId);
    }
}
