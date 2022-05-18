namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyBalancedParticipation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Body.Participation;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WithNullParticipation : Specification<UpdateBodyBalancedParticipationCommandHandler, UpdateBodyBalancedParticipation>
    {
        private Guid _bodyId;

        public WithNullParticipation(ITestOutputHelper helper) : base(helper) { }

        protected override IUser User
            => new UserBuilder().Build();

        protected override UpdateBodyBalancedParticipationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateBodyBalancedParticipationCommandHandler>>().Object,
                Session);

        protected override IEnumerable<IEvent> Given()
        {
            _bodyId = Guid.NewGuid();

            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
            };
        }

        protected override UpdateBodyBalancedParticipation When()
            => new(
                new BodyId(_bodyId),
                null,
                "remark",
                "exception");

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void SetsUnknownParticipation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyBalancedParticipationChanged>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.BalancedParticipationObligatory.Should().BeNull();
            bodyBalancedParticipationChanged.BalancedParticipationExtraRemark.Should().Be("remark");
            bodyBalancedParticipationChanged.BalancedParticipationExceptionMeasure.Should().Be("exception");

            bodyBalancedParticipationChanged.PreviousBalancedParticipationObligatory.Should().BeNull();
            bodyBalancedParticipationChanged.PreviousBalancedParticipationExtraRemark.Should().BeNull();
            bodyBalancedParticipationChanged.PreviousBalancedParticipationExceptionMeasure.Should().BeNull();
        }
    }
}
