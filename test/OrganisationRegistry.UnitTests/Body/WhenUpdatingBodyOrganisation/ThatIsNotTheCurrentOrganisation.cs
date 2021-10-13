namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyOrganisation
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
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared;
    using Xunit;
    using Xunit.Abstractions;

    public class ThatIsNotTheCurrentOrganisation : Specification<Body, BodyCommandHandlers, UpdateBodyOrganisation>
    {
        private Guid _bodyId;
        private Guid _bodyOrganisationId;
        private Guid _otherOrganisationId;
        private Guid _previousOrganisationId;
        private Guid _otherBodyOrganisationId;

        protected override BodyCommandHandlers BuildHandler()
        {
            return new BodyCommandHandlers(
                new Mock<ILogger<BodyCommandHandlers>>().Object,
                Session,
                new DateTimeProviderStub(DateTime.Today),
                new SequentialBodyNumberGenerator(),
                Mock.Of<IUniqueBodyNumberValidator>(),
                Mock.Of<IBodySeatNumberGenerator>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _bodyId = Guid.NewGuid();
            _previousOrganisationId = Guid.NewGuid();
            _otherOrganisationId = Guid.NewGuid();
            _bodyOrganisationId = Guid.NewGuid();
            _otherBodyOrganisationId = Guid.NewGuid();
            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
                new OrganisationCreated(_previousOrganisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new OrganisationCreated(_otherOrganisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new BodyOrganisationAdded(_bodyId, _bodyOrganisationId, "bodyName", _previousOrganisationId, "orgName",
                    null, null),
                new BodyOrganisationAdded(_bodyId, _otherBodyOrganisationId, "other body name", _otherOrganisationId, "other orgName",
                    DateTime.MinValue, DateTime.MinValue),
                new BodyAssignedToOrganisation(_bodyId, "Body", _previousOrganisationId, "orgName", _bodyOrganisationId)
            };
        }

        protected override UpdateBodyOrganisation When()
        {
            return new UpdateBodyOrganisation(
                new BodyId(_bodyId),
                new BodyOrganisationId(_otherBodyOrganisationId),
                new OrganisationId(_otherOrganisationId),
                new Period(new ValidFrom(DateTime.MaxValue), new ValidTo(DateTime.MaxValue)));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void UpdatesTheBodyOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_otherOrganisationId);
        }

        public ThatIsNotTheCurrentOrganisation(ITestOutputHelper helper) : base(helper) { }
    }
}
