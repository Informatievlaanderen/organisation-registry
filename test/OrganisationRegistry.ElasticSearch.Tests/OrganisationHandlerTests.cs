namespace OrganisationRegistry.ElasticSearch.Tests
{
    using Bodies;
    using Body.Events;
    using FluentAssertions;
    using Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Projections.Infrastructure;
    using Scenario;
    using Xunit;
    using System;
    using Organisation.Events;
    using Organisations;
    using Projections.Organisations;

    [Collection(nameof(ElasticSearchFixture))]
    public class OrganisationHandlerTests
    {
        private readonly ElasticSearchFixture _fixture;
        private readonly Organisation _handler;

        public OrganisationHandlerTests(ElasticSearchFixture fixture)
        {
            _fixture = fixture;
            _handler = new Organisation(
                logger: _fixture.LoggerFactory.CreateLogger<Organisation>(),
                elastic: _fixture.Elastic,
                elasticSearchOptions: _fixture.ElasticSearchOptions);
        }

        [EnvVarIgnoreFact]
        public void InitializeProjection_CreatesIndex()
        {
            var scenario = new OrganisationScenario(Guid.NewGuid());

            Handle(scenario.Create<InitialiseProjection>());

            var indices = _fixture.Elastic.ReadClient.GetIndex(_fixture.ElasticSearchOptions.Value.OrganisationsReadIndex).Indices;
            indices.Should().NotBeEmpty();
        }

        [EnvVarIgnoreFact]
        public void OrganisationCreated_CreatesDocument()
        {
            var scenario = new BodyScenario(Guid.NewGuid());

            var initialiseProjection = scenario.Create<InitialiseProjection>();
            var organisationCreated = scenario.Create<OrganisationCreated>();

            Handle(
                initialiseProjection,
                organisationCreated);

            var organisation = _fixture.Elastic.ReadClient.Get<OrganisationDocument>(organisationCreated.OrganisationId);

            organisation.Source.Name.Should().Be(organisationCreated.Name);
            organisation.Source.ShortName.Should().Be(organisationCreated.ShortName);
            organisation.Source.Description.Should().Be(organisationCreated.Description);
        }

        private void Handle(params IEvent[] envelopes)
        {
            foreach (var envelope in envelopes)
            {
                _handler.Handle(null, null, (dynamic)envelope.ToEnvelope());
            }
        }
    }
}
