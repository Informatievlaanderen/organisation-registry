namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Purpose;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAnActiveOrganisationsValidityChangesToThePast : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfo>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreatedTestDataBuilder;
        private DateTime _yesterday;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today), Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationCreatedTestDataBuilder = new OrganisationCreatedTestDataBuilder(new SequentialOvoNumberGenerator());
            _yesterday = DateTime.Today.AddDays(-1);

            return new List<IEvent>
            {
                _organisationCreatedTestDataBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedTestDataBuilder.Id)
            };
        }

        protected override UpdateOrganisationInfo When()
        {
            return new UpdateOrganisationInfo(
                _organisationCreatedTestDataBuilder.Id,
                "Test",
                "testing",
                "",
                new List<PurposeId>(),
                false,
                new ValidFrom(_yesterday),
                new ValidTo(_yesterday));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationInfoUpdated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            var organisationBecameInactive = PublishedEvents[1].UnwrapBody<OrganisationBecameInactive>();
            organisationBecameInactive.Should().NotBeNull();
        }

        public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper) { }
    }
}
