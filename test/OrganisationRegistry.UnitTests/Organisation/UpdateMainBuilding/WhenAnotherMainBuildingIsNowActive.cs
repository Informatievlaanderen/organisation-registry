namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainBuilding;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Building.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Xunit.Abstractions;

public class WhenAnotherMainBuildingIsNowActive :
    Specification<UpdateMainBuildingCommandHandler, UpdateMainBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingAId;
    private readonly Guid _buildingBId;
    private readonly Guid _organisationBuildingAId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenAnotherMainBuildingIsNowActive(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _buildingAId = Guid.NewGuid();
        _buildingBId = Guid.NewGuid();
        _organisationBuildingAId = Guid.NewGuid();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);
    }

    protected override UpdateMainBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateMainBuildingCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

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
            new BuildingCreated(_buildingAId, "Gebouw A", 12345), new BuildingCreated(_buildingBId, "Gebouw B", 12345),
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

    private UpdateMainBuilding UpdateMainBuildingCommand
        => new(new OrganisationId(_organisationId));

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events).When(UpdateMainBuildingCommand, UserBuilder.User())
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task ClearsTheMainBuilding()
    {
        await Given(Events).When(UpdateMainBuildingCommand, UserBuilder.User()).Then();

        PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
    }

    [Fact]
    public async Task AssignsTheNewBuilding()
    {
        await Given(Events).When(UpdateMainBuildingCommand, UserBuilder.User()).Then();

        var mainBuildingAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<MainBuildingAssignedToOrganisation>();
        mainBuildingAssignedToOrganisation.Should().NotBeNull();
        mainBuildingAssignedToOrganisation.MainBuildingId.Should().Be(_buildingBId);
    }
}
