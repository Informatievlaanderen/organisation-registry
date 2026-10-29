namespace OrganisationRegistry.UnitTests.Body.WhenRemovingABodyMandate;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// The only caller of <see cref="RemoveBodyMandate"/> is the
/// SyncRemovedItemsService background job, acting as
/// <see cref="WellknownUsers.SyncRemovedItemsService"/>. That well-known user
/// must hold an unrestricted <see cref="Permission.BodiesCanManageMandates"/>
/// grant, otherwise scheduled body mandate removals would always fail
/// authorization.
/// </summary>
public class WhenRemovingABodyMandateAsSyncRemovedItemsService : Specification<RemoveBodyMandateCommandHandler, RemoveBodyMandate>
{
    private readonly Fixture _fixture;
    private readonly Guid _bodyId;
    private readonly Guid _bodySeatId;
    private readonly Guid _bodyMandateId;
    private readonly Guid _organisationId;
    private readonly Guid _functionTypeId;

    public WhenRemovingABodyMandateAsSyncRemovedItemsService(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _bodyId = _fixture.Create<Guid>();
        _bodySeatId = _fixture.Create<Guid>();
        _bodyMandateId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _functionTypeId = _fixture.Create<Guid>();
    }

    protected override RemoveBodyMandateCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<RemoveBodyMandateCommandHandler>>().Object,
            session);

    private IEvent[] Events
        => new IEvent[]
        {
            new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
            new BodySeatAdded(
                _bodyId,
                _bodySeatId,
                "Seat",
                "1.1",
                _fixture.Create<Guid>(),
                "Seat type",
                1,
                true,
                false,
                true,
                null,
                null),
            new AssignedFunctionTypeToBodySeat(
                _bodyId,
                _bodyMandateId,
                _bodySeatId,
                "1.1",
                "Seat",
                1,
                _organisationId,
                "Organisation",
                _functionTypeId,
                "Function type",
                null,
                null),
        };

    private RemoveBodyMandate RemoveBodyMandateCommand
        => new(
            new BodyId(_bodyId),
            new BodySeatId(_bodySeatId),
            new BodyMandateId(_bodyMandateId));

    [Fact]
    public async Task Succeeds()
    {
        await Given(Events)
            .When(RemoveBodyMandateCommand, WellknownUsers.SyncRemovedItemsService)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
}
