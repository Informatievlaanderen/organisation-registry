namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainBuilding
{
    using System;
    using System.Collections.Generic;
    using Building.Events;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit.Abstractions;

    public class WhenMainBuildingIsStillActive : Specification<Organisation, OrganisationCommandHandlers, UpdateMainBuilding>
    {
        private OrganisationId _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;

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
            _buildingId = Guid.NewGuid();
            _organisationBuildingId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new BuildingCreated(_buildingId, "Gebouw A", 12345),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingId, _buildingId, "Gebouw A", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
            };
        }

        protected override UpdateMainBuilding When()
        {
            return new UpdateMainBuilding(_organisationId);
        }

        protected override int ExpectedNumberOfEvents => 0;

        public WhenMainBuildingIsStillActive(ITestOutputHelper helper) : base(helper) { }
    }
}
