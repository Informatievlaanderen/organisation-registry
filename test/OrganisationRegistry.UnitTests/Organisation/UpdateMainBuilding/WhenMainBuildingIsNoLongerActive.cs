namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainBuilding;

using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Building.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit.Abstractions;

public class WhenMainBuildingIsNoLongerActive : OldSpecification2<UpdateMainBuildingCommandHandler, UpdateMainBuilding>
{
    private Guid _organisationId;
    private Guid _buildingId;
    private Guid _organisationBuildingId;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    public WhenMainBuildingIsNoLongerActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateMainBuildingCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateMainBuildingCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

    protected override IUser User
        => new UserBuilder().Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();
        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();

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
                true,
                DateTime.Today,
                DateTime.Today),
            new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
        };
    }

    protected override UpdateMainBuilding When()
    {
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

        return new UpdateMainBuilding(new OrganisationId(_organisationId));
    }

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void ClearsTheMainBuilding()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
    }
}
