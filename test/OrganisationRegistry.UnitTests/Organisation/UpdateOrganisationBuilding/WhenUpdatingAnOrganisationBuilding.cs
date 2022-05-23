namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding;

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
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenUpdatingAnOrganisationBuilding : OldSpecification2<UpdateOrganisationBuildingCommandHandler,
        UpdateOrganisationBuilding>
{
    private Guid _organisationId;
    private Guid _buildingId;
    private Guid _organisationBuildingId;
    private bool _isMainBuilding;
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenUpdatingAnOrganisationBuilding(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
            Session,
            new DateTimeProvider()
        );

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
            new BuildingCreated(_buildingId, "Gebouw A", 12345),
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

    protected override UpdateOrganisationBuilding When()
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void UpdatesTheOrganisationBuilding()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

        var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
        organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
        organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
        organisationBuildingAdded.IsMainBuilding.Should().Be(_isMainBuilding);
        organisationBuildingAdded.ValidFrom.Should().Be(_validFrom);
        organisationBuildingAdded.ValidTo.Should().Be(_validTo);
    }
}
