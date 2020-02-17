namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using Body;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using OrganisationRegistry.Infrastructure.Events;

    public class BodyRunner : BaseRunner
    {
        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchBodiesProjection";
        private static readonly string ProjectionFullName = typeof(BodyHandler).FullName;
        private new const string ProjectionName = nameof(BodyHandler);

        private new static readonly Type[] EventHandlers =
        {
            typeof(MemoryCachesMaintainer),
            typeof(BodyHandler)
        };

        private new static readonly Type[] ReactionHandlers = { };

        public BodyRunner(
            ILogger<BodyRunner> logger,
            IOptions<ElasticSearchConfiguration> configuration,
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus) :
            base(
                logger,
                configuration,
                store,
                projectionStates,
                bus,
                ElasticSearchProjectionsProjectionName,
                ProjectionFullName,
                ProjectionName,
                EventHandlers,
                ReactionHandlers)
        {
        }
    }
}
