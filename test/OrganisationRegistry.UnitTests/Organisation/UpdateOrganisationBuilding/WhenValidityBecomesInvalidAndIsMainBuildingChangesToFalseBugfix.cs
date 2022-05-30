namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBuilding;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Building;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Building.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenValidityBecomesInvalidAndIsMainBuildingChangesToFalseBugfix :
    Specification<UpdateOrganisationBuildingCommandHandler, UpdateOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly DateTime? _validTo;
    private readonly DateTime _validFrom;

    public WhenValidityBecomesInvalidAndIsMainBuildingChangesToFalseBugfix(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();

        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _validFrom = new DateTime(1980, 10, 17);
        _validTo = null;
    }

    protected override UpdateOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBuildingCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(new DateTime(2017, 01, 19))
        );

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
                true,
                _validFrom,
                _validTo),
#pragma warning disable CS0618
            new MainBuildingAssignedToOrganisation(_organisationId, _buildingId, _organisationBuildingId)
#pragma warning restore CS0618
        };

    private UpdateOrganisationBuilding UpdateOrganisationBuildingCommand
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            false,
            new ValidFrom(
                new DateTime(
                    1980,
                    10,
                    17)),
            new ValidTo(
                new DateTime(
                    2016,
                    06,
                    16)));

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
        var @event = PublishedEvents[0];
        @event.Should().BeOfType<Envelope<OrganisationBuildingUpdated>>();

        var organisationBuildingAdded = PublishedEvents.First().UnwrapBody<OrganisationBuildingUpdated>();
        organisationBuildingAdded.OrganisationId.Should().Be(_organisationId);
        organisationBuildingAdded.BuildingId.Should().Be(_buildingId);
        organisationBuildingAdded.IsMainBuilding.Should().Be(false);
        organisationBuildingAdded.ValidFrom.Should().Be(new DateTime(1980, 10, 17));
        organisationBuildingAdded.ValidTo.Should().Be(new DateTime(2016, 06, 16));
    }

    [Fact]
    public async Task ClearsTheMainBuilding()
    {
        await Given(Events).When(UpdateOrganisationBuildingCommand, TestUser.User).Then();

        var @event = PublishedEvents[1];
#pragma warning disable CS0618
        @event.Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
#pragma warning restore CS0618

#pragma warning disable CS0618
        var mainBuildingClearedFromOrganisation = @event.UnwrapBody<MainBuildingClearedFromOrganisation>();
#pragma warning restore CS0618
        mainBuildingClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        mainBuildingClearedFromOrganisation.MainBuildingId.Should().Be(_buildingId);
    }
}
