namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Building;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Building.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenValidityBecomesInvalidAndIsMainBuildingChangesToFalseBugfix : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationBuilding>
    {
        private Guid _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;
        private DateTime? _validTo;
        private DateTime _validFrom;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var dateTimeProvider = new DateTimeProviderStub(new DateTime(2017, 01, 19));

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                dateTimeProvider,
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _buildingId = Guid.NewGuid();
            _organisationBuildingId = Guid.NewGuid();
            _validFrom = new DateTime(1980, 10, 17);
            _validTo = null;

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new BuildingCreated(_buildingId, "Gebouw A", 12345),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingId, _buildingId, "Gebouw A", true, _validFrom, _validTo),
                new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
            };
        }

        protected override UpdateOrganisationBuilding When()
        {
            return new UpdateOrganisationBuilding(
                _organisationBuildingId,
                new OrganisationId(_organisationId),
                new BuildingId(_buildingId),
                false,
                new ValidFrom(new DateTime(1980,
                    10,
                    17)),
                new ValidTo(new DateTime(2016,
                    06,
                    16)));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheOrganisationBuilding()
        {
            var @event = PublishedEvents[0];
            @event.Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

            var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
            organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
            organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
            organisationBuildingAdded.IsMainBuilding.Should().Be(false);
            organisationBuildingAdded.ValidFrom.Should().Be(new DateTime(1980, 10, 17));
            organisationBuildingAdded.ValidTo.Should().Be(new DateTime(2016, 06, 16));
        }

        [Fact]
        public void ClearsTheMainBuilding()
        {
            var @event = PublishedEvents[1];
            @event.Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();

            var mainBuildingClearedFromOrganisation = @event.UnwrapBody<MainBuildingClearedFromOrganisation>();
            mainBuildingClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
            mainBuildingClearedFromOrganisation.MainBuildingId.Should().Be(_buildingId);
        }

        public WhenValidityBecomesInvalidAndIsMainBuildingChangesToFalseBugfix(ITestOutputHelper helper) : base(helper) { }
    }
}
