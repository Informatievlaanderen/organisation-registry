namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding;

using System;
using System.Collections.Generic;
using System.Linq;
using Building;
using Building.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAMainOrganisationBuilding
    : Specification<AddOrganisationBuildingCommandHandler, AddOrganisationBuilding>
{
    private Guid _organisationId;
    private Guid _buildingId;
    private Guid _organisationBuildingId;
    private bool _isMainBuilding;
    private DateTime _validTo;
    private DateTime _validFrom;
    private DateTimeProviderStub _dateTimeProviderStub;

    public WhenAddingAMainOrganisationBuilding(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
    }

    protected override IUser User
        => new UserBuilder().Build();


    protected override AddOrganisationBuildingCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationBuildingCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub
        );


    protected override IEnumerable<IEvent> Given()
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();

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
            new BuildingCreated(_buildingId, "Gebouw A", 1234)
        };
    }

    protected override AddOrganisationBuilding When()
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void AddsAnOrganisationBuilding()
    {
        var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingAdded>();
        organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
        organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
        organisationBuildingAdded.IsMainBuilding.Should().Be(_isMainBuilding);
        organisationBuildingAdded.ValidFrom.Should().Be(_validFrom);
        organisationBuildingAdded.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public void AssignsAMainBuilding()
    {
        var organisationBuildingAdded = PublishedEvents[1].UnwrapBody<MainBuildingAssignedToOrganisation>();
        organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
        organisationBuildingAdded.MainBuildingId.Should().Be(_buildingId);
    }
}
