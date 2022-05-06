namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfoNotLimitedByVlimpers
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Purpose;
    using Purpose.Events;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAnActiveOrganisationsValidityChangesToThePast : Specification<UpdateOrganisationNotLimitedToVlimpersCommandHandler, UpdateOrganisationInfoNotLimitedToVlimpers>
    {
        private readonly OrganisationCreatedBuilder _organisationCreatedBuilder = new(new SequentialOvoNumberGenerator());
        private Guid _purposeId;

        public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper) { }

        protected override UpdateOrganisationNotLimitedToVlimpersCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler>>().Object,
                Session);

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
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
            => new(
                _organisationCreatedBuilder.Id,
                "omschrijving",
                new List<PurposeId> {new PurposeId(_purposeId)},
                true);

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
    }
}
