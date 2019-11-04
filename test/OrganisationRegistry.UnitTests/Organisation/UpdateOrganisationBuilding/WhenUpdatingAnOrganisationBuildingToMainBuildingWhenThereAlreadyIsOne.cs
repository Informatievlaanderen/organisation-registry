namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
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

    public class WhenMakingAnOrganisationBuildingAMainBuildingWhenThereAlreadyIsOne : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationBuilding>
    {
        private Guid _organisationId;
        private Guid _organisationBuildingId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private Guid _buildingBId;
        private Guid _buildingAId;

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
            _organisationId = Guid.NewGuid();

            _organisationBuildingId = Guid.NewGuid();
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);
            _buildingAId = Guid.NewGuid();
            _buildingBId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A", 12345),
                new BuildingCreated(_buildingBId, "Gebouw B", 12345),
                new OrganisationBuildingAdded(_organisationId, Guid.NewGuid(), _buildingAId, "Gebouw A", true, _validFrom, _validTo),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingId, _buildingBId, "Gebouw B", false, _validFrom, _validTo)
            };
        }

        protected override UpdateOrganisationBuilding When()
        {
            return new UpdateOrganisationBuilding(
                _organisationBuildingId,
                new OrganisationId(_organisationId),
                new BuildingId(_buildingBId),
                true,
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

        public WhenMakingAnOrganisationBuildingAMainBuildingWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(helper) { }
    }
}
