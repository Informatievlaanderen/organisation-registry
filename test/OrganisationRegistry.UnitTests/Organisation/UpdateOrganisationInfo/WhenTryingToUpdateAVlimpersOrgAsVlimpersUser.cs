namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToUpdateAVlimpersOrgAsVlimpersUser : Specification<UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
    {
        private readonly OrganisationCreatedBuilder _organisationCreatedBuilder = new(new SequentialOvoNumberGenerator());

        public WhenTryingToUpdateAVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
                Session,
                new DateTimeProviderStub(DateTime.Today));

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.VlimpersBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
            => new List<IEvent>
            {
                _organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedBuilder.Id),
                new OrganisationPlacedUnderVlimpersManagement(_organisationCreatedBuilder.Id)
            };

        protected override UpdateOrganisationInfoLimitedToVlimpers When()
            => new(
                _organisationCreatedBuilder.Id,
                "Test",
                Article.None,
                "testing",
                new ValidFrom(),
                new ValidTo(),
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 4;

        [Fact]
        public void UpdatesOrganisationName()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesShortName()
        {
            var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationValidity()
        {
            var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationValidityUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void UpdatesOrganisationOperationalValidity()
        {
            var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationOperationalValidityUpdated>();
            organisationCreated.Should().NotBeNull();
        }
    }
}
