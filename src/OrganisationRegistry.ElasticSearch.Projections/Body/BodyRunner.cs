namespace OrganisationRegistry.ElasticSearch.Projections.Body;

using System;
using Client;
using Configuration;
using ElasticSearch.Bodies;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ProjectionState;

public class BodyRunner : BaseRunner<BodyDocument>
{
    public const string ElasticSearchProjectionsProjectionName = "ElasticSearchBodiesProjection";
    private static readonly string ProjectionFullName = typeof(BodyHandler).FullName!;
    private new const string ProjectionName = nameof(Body);

    public new static readonly Type[] EventHandlers =
    {
        typeof(BodyHandler),
    };

    public BodyRunner(
        ILogger<BodyRunner> logger,
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
            ElasticSearchProjectionsProjectionName,
            ProjectionFullName,
            ProjectionName,
            EventHandlers,
            elastic,
            bus,
            contextFactory)
    {
        busRegistrar.RegisterEventHandlers(EventHandlers);
    }
}
