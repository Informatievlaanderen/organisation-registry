namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;

    public class OrganisationsRunner : BaseRunner
    {
        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchOrganisationsProjection";
        private static readonly string ProjectionFullName = typeof(Organisation).FullName;
        private new const string ProjectionName = nameof(Organisation);

        public new static readonly Type[] EventHandlers =
        {
            typeof(MemoryCachesMaintainer),
            typeof(Organisation),
            typeof(OrganisationBody),
            typeof(OrganisationBuilding),
            typeof(OrganisationCapacity),
            typeof(OrganisationContact),
            typeof(OrganisationFormalFramework),
            typeof(OrganisationFunction),
            typeof(OrganisationKey),
            typeof(OrganisationLabel),
            typeof(OrganisationLocation),
            typeof(OrganisationOrganisationClassification),
            typeof(OrganisationParent),
            typeof(OrganisationRelation),
            typeof(OrganisationBankAccount),
            typeof(OrganisationOpeningHoursSpecification)
        };

        private new static readonly Type[] ReactionHandlers = {};

        public OrganisationsRunner(
            ILogger<OrganisationsRunner> logger,
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
