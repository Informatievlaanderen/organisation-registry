namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding
{
    using System;
    using System.Collections.Generic;
    using Building;
    using Building.Events;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class
        WhenAddingTheSameBuildingTwice : ExceptionSpecification<AddOrganisationBuildingCommandHandler,
            AddOrganisationBuilding>
    {
        private Guid _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;
        private bool _isMainBuilding;
        private DateTime _validTo;
        private DateTime _validFrom;

        protected override AddOrganisationBuildingCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationBuildingCommandHandler>>().Object,
                Session,
                new DateTimeProvider());

        protected override IUser User
            => new UserBuilder().Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();
            _buildingId = Guid.NewGuid();
            _organisationBuildingId = Guid.NewGuid();
            _isMainBuilding = true;
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

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
                new BuildingCreated(_buildingId, "Gebouw A", 1234),
                new OrganisationBuildingAdded(
                    _organisationId,
                    _organisationBuildingId,
                    _buildingId,
                    "Gebouw A",
                    _isMainBuilding,
                    _validFrom,
                    _validTo)
            };
        }

        protected override AddOrganisationBuilding When()
        {
            return new AddOrganisationBuilding(
                Guid.NewGuid(),
                new OrganisationId(_organisationId),
                new BuildingId(_buildingId),
                _isMainBuilding,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<BuildingAlreadyCoupledToInThisPeriod>();
            Exception.Message.Should().Be("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.");
        }

        public WhenAddingTheSameBuildingTwice(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
