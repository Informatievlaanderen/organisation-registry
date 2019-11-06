namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using Microsoft.Extensions.Logging;
    using Organisations;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;
    using OrganisationRegistry.Infrastructure.Events;

    public class OrganisationsRunner : BaseRunner
    {
        private const string ElasticSearchProjectionsProjectionName = "ElasticSearchOrganisationsProjection";
        private static readonly string ProjectionFullName = typeof(Organisation).FullName;
        private new const string ProjectionName = nameof(Organisation);

        private new static readonly Type[] EventHandlers =
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
            IEventStore store,
            IProjectionStates projectionStates,
            IEventPublisher bus) :
            base(
                logger,
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
