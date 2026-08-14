namespace OrganisationRegistry.ElasticSearch.Projections.Body;

using System;
using Bodies;
using Client;
using ElasticSearch.Bodies;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;

public class IndividualBodyRebuildRunner
    : IndividualRebuildRunnerBase<OrganisationRegistry.Body.Body, BodyDocument, BodyToRebuild>
{
    public IndividualBodyRebuildRunner(
        ILogger<IndividualBodyRebuildRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar)
        : base(logger, store, contextFactory, projectionStates, bus, elastic, busRegistrar)
    {
    }

    protected override string ProjectionStateKey => BodyRunner.ElasticSearchProjectionsProjectionName;
    protected override Type[] EventHandlers => BodyRunner.EventHandlers;

    protected override DbSet<BodyToRebuild> DataToRebuildSet(OrganisationRegistryContext context)
        => context.BodiesToRebuild;

    protected override Guid GetAggregateId(BodyToRebuild item) => item.BodyId;
}
