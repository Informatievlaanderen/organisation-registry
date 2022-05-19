namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding;

using System;
using System.Collections.Generic;
using Building;
using Building.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit.Abstractions;

public class WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne : ExceptionSpecification<
    AddOrganisationBuildingCommandHandler, AddOrganisationBuilding>
{
    private readonly OrganisationId _organisationId = new(Guid.NewGuid());
    private readonly BuildingId _buildingAId = new(Guid.NewGuid());
    private readonly Guid _organisationBuildingId = Guid.NewGuid();
    private readonly bool _isMainBuilding = true;
    private readonly DateTime _validTo = DateTime.Now.AddDays(2);
    private readonly DateTime _validFrom = DateTime.Now.AddDays(1);
    private readonly BuildingId _buildingBId = new(Guid.NewGuid());

    public WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationBuildingCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationBuildingCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder().Build();

    protected override IEnumerable<IEvent> Given()
        => new List<IEvent>
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
            new BuildingCreated(_buildingAId, "Gebouw A", 1234),
            new BuildingCreated(_buildingBId, "Gebouw A", 1234),
            new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingAId,
                "Gebouw A",
                _isMainBuilding,
                _validFrom,
                _validTo)
        };

    protected override AddOrganisationBuilding When()
        => new(
            Guid.NewGuid(),
            _organisationId,
            _buildingBId,
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<OrganisationAlreadyHasAMainBuildingInThisPeriod>();
        Exception?.Message.Should().Be("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.");
    }
}
