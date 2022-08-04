namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit.Abstractions;

public class WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne : Specification<
    AddOrganisationBuildingCommandHandler, AddOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingAId;
    private readonly Guid _organisationBuildingId;
    private readonly bool _isMainBuilding;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly Guid _buildingBId;

    public WhenAddingAMainOrganisationBuildingWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _buildingAId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validTo = DateTime.Now.AddDays(2);
        _validFrom = DateTime.Now.AddDays(1);
        _buildingBId = Guid.NewGuid();
    }

    protected override AddOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationBuildingCommandHandler>>().Object,
            session,
            new DateTimeProvider());

    private IEvent[] Events
        => new IEvent[]
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
                _validTo),
        };

    private AddOrganisationBuilding AddOrganisationBuildingCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new BuildingId(_buildingBId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationBuildingCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationBuildingCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationAlreadyHasAMainBuildingInThisPeriod>()
            .WithMessage("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.");
    }
}
