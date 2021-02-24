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
                new ParameterNameArg<Guid>("bodyId", bodyId),
                new ParameterNameArg<Guid>("lifecyclePhaseTypeId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("bodySeatId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("bodyMandateId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("delegationAssignmentId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("personId", Guid.NewGuid()),
                new ParameterNameArg<Guid>("organisationId", Guid.NewGuid()))
        {
            var functionTypeId = Guid.NewGuid();
            AddCustomization(new ParameterNameArg<Guid>("functionId", functionTypeId));
            AddCustomization(new ParameterNameArg<Guid>("functionTypeId", functionTypeId));
        }
    }
}
