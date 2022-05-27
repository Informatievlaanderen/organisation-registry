namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding;

using System;
using System.Collections.Generic;
using System.Linq;
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
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnActiveMainOrganisationBuildingToAnInactiveOne :
    Specification<UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly bool _isMainBuilding;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProvider;


    public WhenUpdatingAnActiveMainOrganisationBuildingToAnInactiveOne(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProvider = new DateTimeProviderStub(DateTime.Today);
        _organisationId = Guid.NewGuid();

        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validFrom = _dateTimeProvider.Today;
        _validTo = _dateTimeProvider.Today;
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
            session,
            _dateTimeProvider);

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
            new BuildingCreated(_buildingId, "Gebouw A", 12345), new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingId,
                "Gebouw A",
                _isMainBuilding,
                _validFrom,
                _validTo),
            new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
        };

    private UpdateOrganisationBuilding UpdateOrganisationBuildingCommand
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom.AddYears(1)),
            new ValidTo(_validTo.AddYears(1)));


    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationBuilding()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

        var organisationBuildingUpdated = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
        organisationBuildingUpdated.OrganisationId.Should().Be(_organisationId);
        organisationBuildingUpdated.BuildingId.Should().Be(_buildingId);
        organisationBuildingUpdated.IsMainBuilding.Should().Be(_isMainBuilding);
        organisationBuildingUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
        organisationBuildingUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
    }

    [Fact]
    public async Task ClearsTheMainBuilding()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).Then();
        PublishedEvents[1].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();

        var organisationBuildingUpdated = PublishedEvents[1].UnwrapBody<MainBuildingClearedFromOrganisation>();
        organisationBuildingUpdated.OrganisationId.Should().Be(_organisationId);
        organisationBuildingUpdated.MainBuildingId.Should().Be(_buildingId);
    }
}
