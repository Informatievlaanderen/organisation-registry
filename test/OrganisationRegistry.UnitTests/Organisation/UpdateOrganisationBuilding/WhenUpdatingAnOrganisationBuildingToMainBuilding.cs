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

public class WhenUpdatingAnOrganisationBuildingToMainBuilding :
    Specification<UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly bool _isMainBuilding;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenUpdatingAnOrganisationBuildingToMainBuilding(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();

        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validFrom = _dateTimeProviderStub.Today.AddDays(0);
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
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
            new BuildingCreated(_buildingId, "Gebouw A", 12345), new OrganisationBuildingAdded(
                _organisationId,
                _organisationBuildingId,
                _buildingId,
                "Gebouw A",
                false,
                _validFrom,
                _validTo)
        };

    private UpdateOrganisationBuilding UpdateOrganisationBuildingCommand
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationBuilding()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

        var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
        organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
        organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
        organisationBuildingAdded.IsMainBuilding.Should().Be(_isMainBuilding);
        organisationBuildingAdded.ValidFrom.Should().Be(_validFrom);
        organisationBuildingAdded.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public async Task AssignsTheMainBuilding()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).Then();
        var mainBuildingAssignedToOrganisation =
#pragma warning disable CS0618
            PublishedEvents[1].UnwrapBody<MainBuildingAssignedToOrganisation>();
#pragma warning restore CS0618
        mainBuildingAssignedToOrganisation.OrganisationBuildingId.Should().Be(_organisationBuildingId);
        mainBuildingAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
        mainBuildingAssignedToOrganisation.MainBuildingId.Should().Be(_buildingId);
    }
}
