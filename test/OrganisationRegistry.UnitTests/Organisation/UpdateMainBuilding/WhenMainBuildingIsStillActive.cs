namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainBuilding;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Building.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenMainBuildingIsStillActive : Specification<UpdateMainBuildingCommandHandler, UpdateMainBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;

    public WhenMainBuildingIsStillActive(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
    }

    protected override UpdateMainBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateMainBuildingCommandHandler>>().Object,
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
            new BuildingCreated(_buildingId, "Gebouw A", 12345), new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingId,
                "Gebouw A",
                true,
                DateTime.Today,
                DateTime.Today),
            new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
        };

    private UpdateMainBuilding UpdateMainBuildingCommand
        => new(new OrganisationId(_organisationId));

    [Fact]
   public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateMainBuildingCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
