namespace OrganisationRegistry.ElasticSearch.Projections.People;

using System;
using Cache;
using Client;
using Configuration;
using ElasticSearch.People;
using Handlers;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ProjectionState;

public class PeopleRunner : BaseRunner<PersonDocument>
{
    private const string ElasticSearchProjectionsProjectionName = "ElasticSearchPeopleProjection";
    private static readonly string ProjectionFullName = typeof(Person).FullName!;
    private new const string ProjectionName = nameof(Person);

    public new static readonly Type[] EventHandlers =
    {
        typeof(CachedOrganisationForBodies),
        typeof(Person),
        typeof(PersonFunction),
        typeof(PersonCapacity),
        typeof(PersonMandate),
    };

    public PeopleRunner(
        ILogger<PeopleRunner> logger,
        IOptions<ElasticSearchConfiguration> configuration,
        IEventStore store,
        IProjectionStates projectionStates,
        Elastic elastic,
        ElasticBus bus,
        ElasticBusRegistrar busRegistrar,
        IContextFactory contextFactory) :
        base(
            logger,
            configuration,
            store,
            projectionStates,
            EventHandlers,
            elastic,
            bus,
            contextFactory,
            new ProjectionName(ElasticSearchProjectionsProjectionName, ProjectionFullName, ProjectionName))
    {
        busRegistrar.RegisterEventHandlers(EventHandlers);
    }
}
