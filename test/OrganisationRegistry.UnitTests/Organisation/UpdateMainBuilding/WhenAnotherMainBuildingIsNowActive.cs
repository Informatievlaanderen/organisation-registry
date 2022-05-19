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

public class WhenAnotherMainBuildingIsNowActive :
    Specification<UpdateMainBuildingCommandHandler, UpdateMainBuilding>
{
    private Guid _organisationId;
    private Guid _buildingAId;
    private Guid _buildingBId;
    private Guid _organisationBuildingAId;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    public WhenAnotherMainBuildingIsNowActive(ITestOutputHelper helper) : base(helper)
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
        _buildingAId = Guid.NewGuid();
        _buildingBId = Guid.NewGuid();

        _organisationBuildingAId = Guid.NewGuid();
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
            new BuildingCreated(_buildingAId, "Gebouw A", 12345),
            new BuildingCreated(_buildingBId, "Gebouw B", 12345),
            new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingAId,
                _buildingAId,
                "Gebouw A",
                true,
                DateTime.Today,
                DateTime.Today),
            new MainBuildingAssignedToOrganisation(_organisationId, _buildingAId, _organisationBuildingAId),
            new OrganisationBuildingAdded(
                _organisationId,
                Guid.NewGuid(),
                _buildingBId,
                "Gebouw B",
                true,
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(1))
        };
    }

    protected override UpdateMainBuilding When()
    {
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

        return new UpdateMainBuilding(new OrganisationId(_organisationId));
    }

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void ClearsTheMainBuilding()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
    }

    [Fact]
    public void AssignsTheNewBuilding()
    {
        var mainBuildingAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<MainBuildingAssignedToOrganisation>();
        mainBuildingAssignedToOrganisation.Should().NotBeNull();
        mainBuildingAssignedToOrganisation.MainBuildingId.Should().Be(_buildingBId);
    }
}
