namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using Client;
using ElasticSearch.Organisations;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;

public class IndividualRebuildRunner
    : IndividualRebuildRunnerBase<OrganisationRegistry.Organisation.Organisation, OrganisationDocument, OrganisationToRebuild>
{
    public IndividualRebuildRunner(
        ILogger<IndividualRebuildRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar)
        : base(logger, store, contextFactory, projectionStates, bus, elastic, busRegistrar)
    {
    }

    protected override string ProjectionStateKey => OrganisationsRunner.ElasticSearchProjectionsProjectionName;
    protected override Type[] EventHandlers => OrganisationsRunner.EventHandlers;

    protected override DbSet<OrganisationToRebuild> DataToRebuildSet(OrganisationRegistryContext context)
        => context.OrganisationsToRebuild;

    protected override Guid GetAggregateId(OrganisationToRebuild item) => item.OrganisationId;
}
