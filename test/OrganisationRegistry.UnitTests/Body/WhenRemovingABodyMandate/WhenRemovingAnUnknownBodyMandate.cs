namespace OrganisationRegistry.UnitTests.Body.WhenRemovingABodyMandate;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnUnknownBodyMandate : Specification<RemoveBodyMandateCommandHandler, RemoveBodyMandate>
{
    private readonly Fixture _fixture;
    private readonly Guid _bodyId;
    private readonly Guid _bodySeatId;
    private readonly Guid _unknownBodyMandateId;

    public WhenRemovingAnUnknownBodyMandate(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _bodyId = _fixture.Create<Guid>();
        _bodySeatId = _fixture.Create<Guid>();
        _unknownBodyMandateId = _fixture.Create<Guid>();
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
        };

    private RemoveBodyMandate RemoveBodyMandateCommand
        => new(
            new BodyId(_bodyId),
            new BodySeatId(_bodySeatId),
            new BodyMandateId(_unknownBodyMandateId));

    [Fact]
    public async Task PublishesNoEvents()
        => await Given(Events)
            .When(RemoveBodyMandateCommand, TestUser.OrgaanBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
}
