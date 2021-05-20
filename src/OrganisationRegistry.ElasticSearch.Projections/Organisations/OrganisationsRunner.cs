namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using App.Metrics;
    using Client;
    using Configuration;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.Infrastructure;
    using SqlServer.ProjectionState;

    public class OrganisationsRunner : BaseRunner<OrganisationDocument>
    {
        public const string ElasticSearchProjectionsProjectionName = "ElasticSearchOrganisationsProjection";
        private static readonly string ProjectionFullName = typeof(Organisation).FullName;
        private new const string ProjectionName = nameof(Organisation);

        public new static readonly Type[] EventHandlers =
        {
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

        public OrganisationsRunner(
            ILogger<OrganisationsRunner> logger,
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
}
