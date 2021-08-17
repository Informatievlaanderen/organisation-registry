namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
    using Building;
    using Building.Events;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Xunit.Abstractions;

    public class WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne : ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationBuilding>
    {
        private OrganisationId _organisationId;
        private BuildingId _buildingAId;
        private Guid _organisationBuildingId;
        private bool _isMainBuilding;
        private DateTime _validTo;
        private DateTime _validFrom;
        private BuildingId _buildingBId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = new OrganisationId(Guid.NewGuid());
            _buildingAId = new BuildingId(Guid.NewGuid());
            _buildingBId = new BuildingId(Guid.NewGuid());
            _organisationBuildingId = Guid.NewGuid();
            _isMainBuilding = true;
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A", 1234),
                new BuildingCreated(_buildingBId, "Gebouw A", 1234),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingId, _buildingAId, "Gebouw A", _isMainBuilding, _validFrom, _validTo)
            };
        }

        protected override AddOrganisationBuilding When()
        {
            return new AddOrganisationBuilding(
                Guid.NewGuid(),
                _organisationId,
                _buildingBId,
                _isMainBuilding,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyHasAMainBuildingInThisPeriodException>();
            Exception.Message.Should().Be("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.");
        }

        public WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(helper) { }
    }
}
