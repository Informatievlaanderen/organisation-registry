namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Building;
using Building.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingTheSameBuildingTwice
    : Specification<AddOrganisationBuildingCommandHandler, AddOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly bool _isMainBuilding;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenAddingTheSameBuildingTwice(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
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
            new BuildingCreated(_buildingId, "Gebouw A", 1234),
            new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingId,
                "Gebouw A",
                _isMainBuilding,
                _validFrom,
                _validTo)
        };


    private AddOrganisationBuilding AddOrganisationBuildingCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationBuildingCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }


    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationBuildingCommand, TestUser.User)
            .ThenThrows<BuildingAlreadyCoupledToInThisPeriod>()
            .WithMessage("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
