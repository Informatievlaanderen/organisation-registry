namespace OrganisationRegistry.ElasticSearch.Projections.People
{
    using System;
    using Cache;
    using Configuration;
    using Handlers;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;

    public class PeopleRunner : BaseRunner
    {
        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchPeopleProjection";
        private static readonly string ProjectionFullName = typeof(Person).FullName;
        private new const string ProjectionName = nameof(Person);

        public new static readonly Type[] EventHandlers =
        {
            typeof(MemoryCachesMaintainer),
            typeof(CachedOrganisationForBodies),
            typeof(Person),
            typeof(PersonFunction),
            typeof(PersonCapacity),
            typeof(PersonMandate)
        };

        private new static readonly Type[] ReactionHandlers = { };

        public PeopleRunner(
            ILogger<PeopleRunner> logger,
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
