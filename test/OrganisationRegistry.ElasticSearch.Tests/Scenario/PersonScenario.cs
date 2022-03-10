namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using System.Collections.Generic;
    using Organisation.Events;
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

        public OrganisationCapacityAdded CreateOrganisationCapacityAdded(Guid personId, Guid organisationId)
            => new(
                organisationId,
                Create<Guid>(),
                Create<Guid>(),
                Create<string>(),
                personId,
                Create<string>(),
                Create<Guid>(),
                Create<string>(),
                Create<Guid>(),
                Create<string>(),
                new Dictionary<Guid, string>(),
                Create<DateTime?>(),
                Create<DateTime?>()
            );
    }
}
