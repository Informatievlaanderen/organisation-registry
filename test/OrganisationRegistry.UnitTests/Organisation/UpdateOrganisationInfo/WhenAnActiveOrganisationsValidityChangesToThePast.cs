namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Purpose;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class
        WhenAnActiveOrganisationsValidityChangesToThePast : Specification<UpdateOrganisationCommandHandler,
            UpdateOrganisationInfo>
    {
        private readonly DateTime _yesterday = DateTime.Today.AddDays(-1);
        private Guid _organisationId;

        public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override UpdateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
                Session,
                new DateTimeProviderStub(DateTime.Today));

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

            _organisationId = organisationCreatedBuilder.Id;

            return new List<IEvent>
            {
                organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(organisationCreatedBuilder.Id)
            };
        }

        protected override UpdateOrganisationInfo When()
            => new(
                new OrganisationId(_organisationId),
                "Test",
                Article.None,
                "testing",
                "",
                new List<PurposeId>(),
                false,
                new ValidFrom(_yesterday),
                new ValidTo(_yesterday),
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 6;

        [Fact]
        public void UpdatesOrganisationName()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationShortName()
        {
            var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationDescription()
        {
            var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationDescriptionUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationValidity()
        {
            var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationValidityUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationOperationalValidity()
        {
            var organisationCreated = PublishedEvents[4].UnwrapBody<OrganisationOperationalValidityUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            var organisationBecameInactive = PublishedEvents[5].UnwrapBody<OrganisationBecameInactive>();
            organisationBecameInactive.Should().NotBeNull();
        }
    }
}
