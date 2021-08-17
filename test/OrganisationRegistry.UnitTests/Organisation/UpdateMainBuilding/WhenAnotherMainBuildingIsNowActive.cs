namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainBuilding
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Building.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Xunit.Abstractions;

    public class WhenAnotherMainBuildingIsNowActive :
        Specification<Organisation, OrganisationCommandHandlers, UpdateMainBuilding>
    {
        private OrganisationId _organisationId;
        private Guid _buildingAId;
        private Guid _buildingBId;
        private Guid _organisationBuildingAId;
        private DateTimeProviderStub _dateTimeProviderStub;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            return new OrganisationCommandHandlers(new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = new OrganisationId(Guid.NewGuid());
            _buildingAId = Guid.NewGuid();
            _buildingBId = Guid.NewGuid();

            _organisationBuildingAId = Guid.NewGuid();
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new BuildingCreated(_buildingAId, "Gebouw A", 12345),
                new BuildingCreated(_buildingBId, "Gebouw B", 12345),
                new OrganisationBuildingAdded(_organisationId, _organisationBuildingAId, _buildingAId, "Gebouw A", true, DateTime.Today, DateTime.Today),
                new MainBuildingAssignedToOrganisation(_organisationId, _buildingAId, _organisationBuildingAId),
                new OrganisationBuildingAdded(_organisationId, Guid.NewGuid(), _buildingBId, "Gebouw B", true, DateTime.Today.AddDays(1), DateTime.Today.AddDays(1))
            };
        }

        protected override UpdateMainBuilding When()
        {
            _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

            return new UpdateMainBuilding(_organisationId);
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void ClearsTheMainBuilding()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
        }

        [Fact]
        public void AssignsTheNewBuilding()
        {
            var mainBuildingAssignedToOrganisation = PublishedEvents[1].UnwrapBody<MainBuildingAssignedToOrganisation>();
            mainBuildingAssignedToOrganisation.Should().NotBeNull();
            mainBuildingAssignedToOrganisation.MainBuildingId.Should().Be(_buildingBId);
        }

        public WhenAnotherMainBuildingIsNowActive(ITestOutputHelper helper) : base(helper) { }
    }
}
