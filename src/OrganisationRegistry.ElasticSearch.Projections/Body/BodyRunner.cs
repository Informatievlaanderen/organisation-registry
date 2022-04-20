namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using App.Metrics;
    using Bodies;
    using Client;
    using Configuration;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.ProjectionState;

    public class BodyRunner : BaseRunner<BodyDocument>
    {
        public const string ElasticSearchProjectionsProjectionName = "ElasticSearchBodiesProjection";
        private static readonly string ProjectionFullName = typeof(BodyHandler).FullName!;
        private new const string ProjectionName = nameof(Body);

        public new static readonly Type[] EventHandlers =
        {
            typeof(BodyHandler)
        };

        public BodyRunner(
            ILogger<BodyRunner> logger,
            IOptions<ElasticSearchConfiguration> configuration,
            IEventStore store,
            IProjectionStates projectionStates,
            Elastic elastic,
            ElasticBus bus,
            ElasticBusRegistrar busRegistrar,
            IMetricsRoot metrics) :
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
}
