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

public class WhenMainBuildingIsNoLongerActive : Specification<UpdateMainBuildingCommandHandler, UpdateMainBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenMainBuildingIsNoLongerActive(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
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
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateMainBuildingCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task ClearsTheMainBuilding()
    {
        await Given(Events).When(UpdateMainBuildingCommand, TestUser.User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<MainBuildingClearedFromOrganisation>>();
    }
}
