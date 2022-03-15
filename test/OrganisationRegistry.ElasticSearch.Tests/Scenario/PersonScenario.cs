namespace OrganisationRegistry.ElasticSearch.Tests.Scenario
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Client;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Organisation.Events;
    using Projections.People.Handlers;
    using Specimen;
    using SqlServer;

    /// <summary>
    /// Sets up a fixture which uses the same organisationId for all events
    /// </summary>
    public class PersonScenario : ScenarioBase<TestPersonHandler>
    {
        public PersonScenario(Guid personId) :
            base(
                new ParameterNameArg<Guid>("personId", personId),
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

    public class TestPersonHandler : Person
    {
        public TestPersonHandler(ILogger<Person> logger, Elastic elastic, IContextFactory contextFactory, IOptions<ElasticSearchConfiguration> elasticSearchOptions)
            : base(logger, elastic, contextFactory, elasticSearchOptions)
        {
        }

        protected override Task ClearConfigurations()
            => Task.CompletedTask;
    }
}
