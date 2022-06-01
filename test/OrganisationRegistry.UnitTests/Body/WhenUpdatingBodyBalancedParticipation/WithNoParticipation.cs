namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyBalancedParticipation;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WithNoParticipation : Specification<UpdateBodyBalancedParticipationCommandHandler,
    UpdateBodyBalancedParticipation>
{
    private readonly Guid _bodyId;

    public WithNoParticipation(ITestOutputHelper helper) : base(helper)
    {
        _bodyId = Guid.NewGuid();
    }

    protected override UpdateBodyBalancedParticipationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateBodyBalancedParticipationCommandHandler>>().Object,
            session);

    private IEvent[] Events
        => new IEvent[]
            { new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now) };

    private UpdateBodyBalancedParticipation UpdateBodyBalancedParticipationCommand
        => new(
            new BodyId(_bodyId),
            false,
            "remark",
            "exception");

    [Fact]
    public async Task Publishes1Event()
    {
        await Given(Events).When(UpdateBodyBalancedParticipationCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task SetsNoParticipation()
    {
        await Given(Events).When(UpdateBodyBalancedParticipationCommand, TestUser.User).Then();
        var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyBalancedParticipationChanged>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.BalancedParticipationObligatory.Should().BeFalse();
        bodyBalancedParticipationChanged.BalancedParticipationExtraRemark.Should().Be("remark");
        bodyBalancedParticipationChanged.BalancedParticipationExceptionMeasure.Should().Be("exception");

        bodyBalancedParticipationChanged.PreviousBalancedParticipationObligatory.Should().BeNull();
        bodyBalancedParticipationChanged.PreviousBalancedParticipationExtraRemark.Should().BeNull();
        bodyBalancedParticipationChanged.PreviousBalancedParticipationExceptionMeasure.Should().BeNull();
    }
}
