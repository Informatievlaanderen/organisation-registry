namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Building;
    using Building.Events;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAMainOrganisationBuildingToRegularBuilding : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationBuilding>
    {
        private Guid _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;
        private bool _isMainBuilding;
        private DateTime _validTo;
        private DateTime _validFrom;
        private readonly DateTimeProviderStub _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _buildingId = Guid.NewGuid();
            _organisationBuildingId = Guid.NewGuid();
            _isMainBuilding = false;
            _validFrom = _dateTimeProviderStub.Today.AddDays(0);
            _validTo = _dateTimeProviderStub.Today.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
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
                _isMainBuilding,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheOrganisationBuilding()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

            var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
            organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
            organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
            organisationBuildingAdded.IsMainBuilding.Should().Be(_isMainBuilding);
            organisationBuildingAdded.ValidFrom.Should().Be(_validFrom);
            organisationBuildingAdded.ValidTo.Should().Be(_validTo);
        }

        [Fact]
        public void ClearsTheMainBuilding()
        {
            var mainBuildingAssignedToOrganisation = PublishedEvents[1].UnwrapBody<MainBuildingClearedFromOrganisation>();
            mainBuildingAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
            mainBuildingAssignedToOrganisation.MainBuildingId.Should().Be(_buildingId);
        }

        public WhenUpdatingAMainOrganisationBuildingToRegularBuilding(ITestOutputHelper helper) : base(helper) { }
    }
}
