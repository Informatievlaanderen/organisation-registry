namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBuilding;

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

public class WhenAddingAMainOrganisationBuilding
    : Specification<AddOrganisationBuildingCommandHandler, AddOrganisationBuilding>
{
    private readonly Guid _organisationId;
    private readonly Guid _buildingId;
    private readonly Guid _organisationBuildingId;
    private readonly bool _isMainBuilding;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenAddingAMainOrganisationBuilding(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _buildingId = Guid.NewGuid();
        _organisationBuildingId = Guid.NewGuid();
        _isMainBuilding = true;
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
    }


    protected override AddOrganisationBuildingCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationBuildingCommandHandler>>().Object,
            session,
            _dateTimeProviderStub
        );


    protected IEvent[] Events
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
            new BuildingCreated(_buildingId, "Gebouw A", 1234)
        };

    protected AddOrganisationBuilding AddOrganisationBuildingCommand
        => new(
            _organisationBuildingId,
            new OrganisationId(_organisationId),
            new BuildingId(_buildingId),
            _isMainBuilding,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events).When(AddOrganisationBuildingCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task AddsAnOrganisationBuilding()
    {
        await Given(Events).When(AddOrganisationBuildingCommand, UserBuilder.User()).Then();

        PublishedEvents
            .First()
            .UnwrapBody<OrganisationBuildingAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationBuildingAdded(
                    _organisationId,
                    _organisationBuildingId,
                    _buildingId,
                    "Gebouw A",
                    _isMainBuilding,
                    _validFrom,
                    _validTo)
            , opt =>
                    opt.Excluding(e => e.Timestamp)
                        .Excluding(e =>e .Version));
    }

    [Fact]
    public async Task AssignsAMainBuilding()
    {
        await Given(Events).When(AddOrganisationBuildingCommand, UserBuilder.User()).Then();

        PublishedEvents[1]
            .UnwrapBody<MainBuildingAssignedToOrganisation>()
            .Should()
            .BeEquivalentTo(
                new MainBuildingAssignedToOrganisation(
                    _organisationId,
                    _buildingId,
                    _organisationBuildingId)
                , opt =>
                    opt.ExcludeEventProperties());
    }
}
