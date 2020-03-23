namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using Projections.Body;
    using Specimen;

    /// <summary>
    /// Sets up a fixture which uses the same bodyId and LifecyclePhaseTypeId for all events
    /// </summary>
    public class BodyScenario : ScenarioBase<BodyHandler>
    {
        public BodyScenario(Guid bodyId) :
            base(
                new ParameterNameArg("bodyId", bodyId),
                new ParameterNameArg("lifecyclePhaseTypeId", Guid.NewGuid()),
                new ParameterNameArg("bodySeatId", Guid.NewGuid()),
                new ParameterNameArg("bodyMandateId", Guid.NewGuid()),
                new ParameterNameArg("delegationAssignmentId", Guid.NewGuid()),
                new ParameterNameArg("personId", Guid.NewGuid()),
                new ParameterNameArg("organisationId", Guid.NewGuid()))
        {
            var functionTypeId = Guid.NewGuid();
            AddCustomization(new ParameterNameArg("functionId", functionTypeId));
            AddCustomization(new ParameterNameArg("functionTypeId", functionTypeId));
        }
    }
}
