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
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;

    using Tests.Shared;
    using Xunit.Abstractions;

    public class WhenMainBuildingIsNoLongerActive : OldSpecification<Organisation, OrganisationCommandHandlers, UpdateMainBuilding>
    {
        private OrganisationId _organisationId;
        private Guid _buildingId;
        private Guid _organisationBuildingId;
        private DateTimeProviderStub _dateTimeProviderStub;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
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
            _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

            return new UpdateMainBuilding(_organisationId);
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void ClearsTheMainBuilding()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
        }

        public WhenMainBuildingIsNoLongerActive(ITestOutputHelper helper) : base(helper) { }
    }
}
