namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyBalancedParticipation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class WithNullParticipation : Specification<Body, BodyCommandHandlers, UpdateBodyBalancedParticipation>
    {
        private Guid _bodyId;

        protected override BodyCommandHandlers BuildHandler()
        {
            return new BodyCommandHandlers(
                new Mock<ILogger<BodyCommandHandlers>>().Object,
                Session,
                Mock.Of<IDateTimeProvider>(),
                new SequentialBodyNumberGenerator(),
                Mock.Of<IUniqueBodyNumberValidator>(),
                Mock.Of<IBodySeatNumberGenerator>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _bodyId = Guid.NewGuid();

            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
            };
        }

        protected override UpdateBodyBalancedParticipation When()
        {
            return new UpdateBodyBalancedParticipation(
                new BodyId(_bodyId),
                null,
                "remark",
                "exception");
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void DefaultsToNoParticipation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyBalancedParticipationChanged>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.BalancedParticipationObligatory.Should().BeFalse();
            bodyBalancedParticipationChanged.BalancedParticipationExtraRemark.Should().Be("remark");
            bodyBalancedParticipationChanged.BalancedParticipationExceptionMeasure.Should().Be("exception");

            bodyBalancedParticipationChanged.PreviousBalancedParticipationObligatory.Should().BeNull();
            bodyBalancedParticipationChanged.PreviousBalancedParticipationExtraRemark.Should().BeNull();
            bodyBalancedParticipationChanged.PreviousBalancedParticipationExceptionMeasure.Should().BeNull();
        }

        public WithNullParticipation(ITestOutputHelper helper) : base(helper) { }
    }
}
