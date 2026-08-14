namespace OrganisationRegistry.ElasticSearch.Projections.People;

using System;
using Client;
using ElasticSearch.People;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;
using SqlServer.Infrastructure;
using SqlServer.ProjectionState;

public class IndividualPersonRebuildRunner
    : IndividualRebuildRunnerBase<OrganisationRegistry.Person.Person, PersonDocument, PersonToRebuild>
{
    public IndividualPersonRebuildRunner(
        ILogger<IndividualPersonRebuildRunner> logger,
        IEventStore store,
        IContextFactory contextFactory,
        IProjectionStates projectionStates,
        ElasticBus bus,
        Elastic elastic,
        ElasticBusRegistrar busRegistrar)
        : base(logger, store, contextFactory, projectionStates, bus, elastic, busRegistrar)
    {
    }

    protected override string ProjectionStateKey => PeopleRunner.ElasticSearchProjectionsProjectionName;
    protected override Type[] EventHandlers => PeopleRunner.EventHandlers;

    protected override DbSet<PersonToRebuild> DataToRebuildSet(OrganisationRegistryContext context)
        => context.PeopleToRebuild;

    protected override Guid GetAggregateId(PersonToRebuild item) => item.PersonId;
}
