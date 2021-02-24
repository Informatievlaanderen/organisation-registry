namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using Projections.Organisations;
    using Specimen;

    /// <summary>
    /// Sets up a fixture which uses the same organisationId for all events
    /// </summary>
    public class PersonScenario : ScenarioBase<Organisation>
    {
        public PersonScenario(Guid personId) :
            base(new ParameterNameArg<Guid>("personId", personId),
                new ParameterNameArg<Guid?>("personId", personId))
        {
        }
    }
}
