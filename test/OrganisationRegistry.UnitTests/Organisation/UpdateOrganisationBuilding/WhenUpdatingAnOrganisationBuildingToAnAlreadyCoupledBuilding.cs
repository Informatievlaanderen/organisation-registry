namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding;

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
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationBuildingToAnAlreadyCoupledBuilding : ExceptionSpecification<
    UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private Guid _organisationId;
    private Guid _buildingAId;
    private Guid _buildingBId;
    private Guid _anotherOrganisationBuildingAddedOrganisationBuildingId;

    public WhenUpdatingAnOrganisationBuildingToAnAlreadyCoupledBuilding(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder().Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();
        _buildingAId = Guid.NewGuid();
        _buildingBId = Guid.NewGuid();
        _anotherOrganisationBuildingAddedOrganisationBuildingId = Guid.NewGuid();

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
            new OrganisationBuildingAdded(
                _organisationId,
                Guid.NewGuid(),
                _buildingAId,
                "Gebouw A",
                false,
                null,
                null),
            new OrganisationBuildingAdded(
                _organisationId,
                _anotherOrganisationBuildingAddedOrganisationBuildingId,
                _buildingBId,
                "Gebouw A",
                false,
                null,
                null)
        };
    }

    protected override UpdateOrganisationBuilding When()
        => new(
            _anotherOrganisationBuildingAddedOrganisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingAId),
            false,
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<BuildingAlreadyCoupledToInThisPeriod>();
        Exception?.Message.Should().Be("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
