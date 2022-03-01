namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Configuration;
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
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfoLimitedToVlimpers>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreatedTestDataBuilder;

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
            var fixture = new Fixture();
            _organisationCreatedTestDataBuilder = new OrganisationCreatedTestDataBuilder(new SequentialOvoNumberGenerator());

            return new List<IEvent>
            {
                _organisationCreatedTestDataBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedTestDataBuilder.Id),
                new OrganisationPlacedUnderVlimpersManagement(_organisationCreatedTestDataBuilder.Id),
                new OrganisationTerminatedV2(_organisationCreatedTestDataBuilder.Id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateTime>(),
                    new FieldsToTerminateV2(null,
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>()),
                    new KboFieldsToTerminateV2(
                        new Dictionary<Guid, DateTime>(),
                        new KeyValuePair<Guid, DateTime>?(),
                        new KeyValuePair<Guid, DateTime>?(),
                        new KeyValuePair<Guid, DateTime>?()
                        ),
                    fixture.Create<bool>(),
                    fixture.Create<DateTime?>()
                    ),
            };
        }

        protected override UpdateOrganisationInfoLimitedToVlimpers When()
        {
            var user = new UserBuilder()
                .AddRoles(Role.VlimpersBeheerder)
                .Build();

            return new UpdateOrganisationInfoLimitedToVlimpers(
                _organisationCreatedTestDataBuilder.Id,
                "Test",
                Article.None,
                "testing",
                new ValidFrom(),
                new ValidTo(),
                new ValidFrom(),
                new ValidTo())
            {
                User = user
            };
        }

        protected override int ExpectedNumberOfEvents => 4;

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

        public WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper) { }
    }
}
