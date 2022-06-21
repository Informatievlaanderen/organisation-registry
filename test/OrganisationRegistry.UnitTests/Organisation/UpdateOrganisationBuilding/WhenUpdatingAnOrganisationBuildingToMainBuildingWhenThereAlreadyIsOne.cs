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

public class WhenMakingAnOrganisationBuildingAMainBuildingWhenThereAlreadyIsOne : Specification<
    UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationBuildingId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly Guid _buildingBId;
    private readonly Guid _buildingAId;

    public WhenMakingAnOrganisationBuildingAMainBuildingWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(
        helper)
    {
        _organisationId = Guid.NewGuid();

        _organisationBuildingId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _buildingAId = Guid.NewGuid();
        _buildingBId = Guid.NewGuid();
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
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
            new BuildingCreated(_buildingAId, "Gebouw A", 12345),
            new BuildingCreated(_buildingBId, "Gebouw B", 12345),
            new OrganisationBuildingAdded(
                _organisationId,
                Guid.NewGuid(),
                _buildingAId,
                "Gebouw A",
                true,
                _validFrom,
                _validTo),
            new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingBId,
                "Gebouw B",
                false,
                _validFrom,
                _validTo),
        };

    private UpdateOrganisationBuilding UpdateOrganisationBuildingCommand
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingBId),
            true,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvent()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User)
            .ThenThrows<OrganisationAlreadyHasAMainBuildingInThisPeriod>()
            .WithMessage("Deze organisatie heeft reeds een hoofdgebouw binnen deze periode.");

    }
}
