namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfoNotLimitedByVlimpers
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Purpose;
    using Purpose.Events;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAnActiveOrganisationsValidityChangesToThePast : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfoNotLimitedToVlimpers>
    {
        private OrganisationCreatedBuilder _organisationCreatedBuilder;
        private DateTime _yesterday;
        private Guid _purposeId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today), Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());
            _yesterday = DateTime.Today.AddDays(-1);

            _purposeId = Guid.NewGuid();
            return new List<IEvent>
            {
                _organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedBuilder.Id),
                new PurposeCreated(_purposeId, "beleidsveld")
            };
        }

        protected override UpdateOrganisationInfoNotLimitedToVlimpers When()
        {
            var user = new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

            return new UpdateOrganisationInfoNotLimitedToVlimpers(
                _organisationCreatedBuilder.Id,
                "omschrijving",
                new List<PurposeId> {new PurposeId(_purposeId)},
                true)
            {
                User = user
            };
        }

        protected override int ExpectedNumberOfEvents => 3;


        [Fact]
        public void UpdatesOrganisationDescription()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationDescriptionUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationValidity()
        {
            var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationPurposesUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationOperationalValidity()
        {
            var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationShowOnVlaamseOverheidSitesUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper) { }
    }
}
