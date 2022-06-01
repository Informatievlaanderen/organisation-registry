namespace OrganisationRegistry.ElasticSearch.Projections.People;

using System;
using App.Metrics;
using Cache;
using Client;
using Configuration;
using ElasticSearch.People;
using Handlers;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Events;
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
        IMetricsRoot metrics,
        ElasticBusRegistrar busRegistrar) :
        base(
            logger,
            configuration,
            store,
            projectionStates,
            ElasticSearchProjectionsProjectionName,
            ProjectionFullName,
            ProjectionName,
            EventHandlers,
            elastic,
            bus,
            metrics)
    {
        busRegistrar.RegisterEventHandlers(EventHandlers);
    }
}
