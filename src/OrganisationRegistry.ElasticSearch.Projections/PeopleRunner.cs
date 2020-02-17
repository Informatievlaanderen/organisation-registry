namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using People.Cache;
    using People.Handlers;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using OrganisationRegistry.Infrastructure.Events;

    public class PeopleRunner : BaseRunner
    {
        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchPeopleProjection";
        private static readonly string ProjectionFullName = typeof(Person).FullName;
        private new const string ProjectionName = nameof(Person);

        private new static readonly Type[] EventHandlers =
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
