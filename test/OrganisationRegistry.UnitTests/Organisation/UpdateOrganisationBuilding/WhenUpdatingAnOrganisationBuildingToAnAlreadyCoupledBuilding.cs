namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding;

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

public class WhenUpdatingAnOrganisationBuildingToAnAlreadyCoupledBuilding : Specification<
    UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingAId;
    private readonly Guid _buildingBId;
    private readonly Guid _anotherOrganisationBuildingAddedOrganisationBuildingId;

    public WhenUpdatingAnOrganisationBuildingToAnAlreadyCoupledBuilding(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _buildingAId = Guid.NewGuid();
        _buildingBId = Guid.NewGuid();
        _anotherOrganisationBuildingAddedOrganisationBuildingId = Guid.NewGuid();
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
            session,
            new DateTimeProvider());

    private IEvent[] Events
        => new IEvent[] {
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
            new BuildingCreated(_buildingAId, "Gebouw A", 12345), new OrganisationBuildingAdded(
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

    private UpdateOrganisationBuilding UpdateOrganisationBuildingCommand
        => new(
            _anotherOrganisationBuildingAddedOrganisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingAId),
            false,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User)
            .ThenThrows<BuildingAlreadyCoupledToInThisPeriod>()
            .WithMessage("Dit gebouw is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
