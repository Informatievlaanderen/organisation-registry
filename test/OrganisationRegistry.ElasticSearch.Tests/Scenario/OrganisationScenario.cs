namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using Organisation.Events;
    using Projections.Organisations;
    using Specimen;

    /// <summary>
    /// Sets up a fixture which uses the same organisationId for all events
    /// </summary>
    public class OrganisationScenario : ScenarioBase<Organisation>
    {
        public OrganisationScenario(Guid organisationId) :
            base(new ParameterNameArg<Guid>("organisationId", organisationId))
        {
            var functionTypeId = Guid.NewGuid();
            AddCustomization(new ParameterNameArg<Guid>("functionId", functionTypeId));
            AddCustomization(new ParameterNameArg<Guid>("functionTypeId", functionTypeId));
        }

        public OrganisationRegulationUpdated CreateOrganisationRegulationUpdated(OrganisationRegulationAdded organisationRegulationAdded)
            => new(
                organisationId: organisationRegulationAdded.OrganisationId,
                organisationRegulationAdded.OrganisationRegulationId,
                Create<Guid>(),
                Create<string>(),
                Create<Guid>(),
                Create<string>(),
                Create<string>(),
                Create<string>(),
                Create<string>(),
                Create<DateTime?>(),
                Create<string>(),
                Create<string>(),
                Create<DateTime?>(),
                Create<DateTime?>(),
                Create<Guid>(),
                Create<string>(),
                Create<Guid>(),
                Create<string>(),
                Create<string>(),
                Create<DateTime?>(),
                Create<DateTime?>(),
                Create<string>(),
                Create<string>(),
                Create<DateTime?>(),
                Create<string>(),
                Create<string>());
    }
}
