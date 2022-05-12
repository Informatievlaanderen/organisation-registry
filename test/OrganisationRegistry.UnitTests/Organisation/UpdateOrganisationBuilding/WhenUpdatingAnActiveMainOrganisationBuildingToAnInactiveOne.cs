namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Building;
    using Building.Events;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class
        WhenUpdatingAnActiveMainOrganisationBuildingToAnInactiveOne : Specification<
            UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
    {
        private Guid _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;
        private bool _isMainBuilding;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProvider;

        protected override UpdateOrganisationBuildingCommandHandler BuildHandler()
        {
            return new UpdateOrganisationBuildingCommandHandler(
                new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
                Session,
                _dateTimeProvider);
        }

        protected override IUser User
            => new UserBuilder().Build();

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Today);
            _organisationId = Guid.NewGuid();

            _buildingId = Guid.NewGuid();
            _organisationBuildingId = Guid.NewGuid();
            _isMainBuilding = true;
            _validFrom = _dateTimeProvider.Today;
            _validTo = _dateTimeProvider.Today;

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "Kind en Gezin",
                    "OVO000012345",
                    "K&G",
                    Article.None,
                    "Kindjes en gezinnetjes",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new BuildingCreated(_buildingId, "Gebouw A", 12345),
                new OrganisationBuildingAdded(
                    _organisationId,
                    _organisationBuildingId,
                    _buildingId,
                    "Gebouw A",
                    _isMainBuilding,
                    _validFrom,
                    _validTo),
                new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
            };
        }

        protected override UpdateOrganisationBuilding When()
        {
            return new UpdateOrganisationBuilding(
                _organisationBuildingId,
                new OrganisationId(_organisationId),
                new BuildingId(_buildingId),
                _isMainBuilding,
                new ValidFrom(_validFrom.AddYears(1)),
                new ValidTo(_validTo.AddYears(1)));
        }

        protected override int ExpectedNumberOfEvents
            => 2;

        [Fact]
        public void UpdatesTheOrganisationBuilding()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

            var organisationBuildingUpdated = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
            organisationBuildingUpdated.OrganisationId.Should().Be(_organisationId);
            organisationBuildingUpdated.BuildingId.Should().Be(_buildingId);
            organisationBuildingUpdated.IsMainBuilding.Should().Be(_isMainBuilding);
            organisationBuildingUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
            organisationBuildingUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
        }

        [Fact]
        public void ClearsTheMainBuilding()
        {
            PublishedEvents[1].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();

            var organisationBuildingUpdated = PublishedEvents[1].UnwrapBody<MainBuildingClearedFromOrganisation>();
            organisationBuildingUpdated.OrganisationId.Should().Be(_organisationId);
            organisationBuildingUpdated.MainBuildingId.Should().Be(_buildingId);
        }

        public WhenUpdatingAnActiveMainOrganisationBuildingToAnInactiveOne(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
