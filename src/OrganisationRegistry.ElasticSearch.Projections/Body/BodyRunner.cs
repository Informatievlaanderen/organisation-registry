namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using Bodies;
    using Client;
    using Configuration;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.ProjectionState;

    public class BodyRunner : BaseRunner<BodyDocument>
    {
        public const string ElasticSearchProjectionsProjectionName = "ElasticSearchBodiesProjection";
        private static readonly string ProjectionFullName = typeof(Body).FullName;
        private new const string ProjectionName = nameof(Projections.Body);

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
                bus)
        {
            busRegistrar.RegisterEventHandlers(EventHandlers);
        }
    }
}
