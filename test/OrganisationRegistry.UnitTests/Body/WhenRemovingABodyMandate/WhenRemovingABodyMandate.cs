namespace OrganisationRegistry.UnitTests.Body.WhenRemovingABodyMandate;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
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

public class WhenRemovingABodyMandate : Specification<RemoveBodyMandateCommandHandler, RemoveBodyMandate>
{
    private readonly Fixture _fixture;
    private readonly Guid _bodyId;
    private readonly Guid _bodySeatId;
    private readonly Guid _bodyMandateId;
    private readonly Guid _organisationId;
    private readonly Guid _functionTypeId;

    public WhenRemovingABodyMandate(ITestOutputHelper helper) : base(helper)
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
    public async Task PublishesOneEvent()
    {
        await Given(Events)
            .When(RemoveBodyMandateCommand, TestUser.OrgaanBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var bodyMandateRemoved = PublishedEvents[0].UnwrapBody<BodyMandateRemoved>();
        bodyMandateRemoved.BodyId.Should().Be(_bodyId);
        bodyMandateRemoved.BodySeatId.Should().Be(_bodySeatId);
        bodyMandateRemoved.BodyMandateId.Should().Be(_bodyMandateId);
    }
}
