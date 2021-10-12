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

    public class WhenUpdatingBodyOrganisationWithDifferentOrganisation : Specification<Body, BodyCommandHandlers, UpdateBodyOrganisation>
    {
        private Guid _bodyId;
        private Guid _bodyOrganisationId;
        private Guid _newOrganisationId;
        private Guid _previousOrganisationId;

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
            _previousOrganisationId = Guid.NewGuid();
            _newOrganisationId = Guid.NewGuid();
            _bodyOrganisationId = Guid.NewGuid();
            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
                new OrganisationCreated(_previousOrganisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new OrganisationCreated(_newOrganisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new BodyOrganisationAdded(_bodyId, _bodyOrganisationId, "bodyName", _previousOrganisationId, "orgName",
                    null, null),
                new BodyAssignedToOrganisation(_bodyId, "Body", _previousOrganisationId, "orgName", _bodyOrganisationId)
            };
        }

        protected override UpdateBodyOrganisation When()
        {
            return new UpdateBodyOrganisation(
                new BodyId(_bodyId),
                new BodyOrganisationId(_bodyOrganisationId),
                new OrganisationId(_newOrganisationId),
                new Period());
        }

        protected override int ExpectedNumberOfEvents => 3;

        [Fact]
        public void UpdatesTheBodyOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_newOrganisationId);
        }

        [Fact]
        public void ClearsThePreviousOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[1].UnwrapBody<BodyClearedFromOrganisation>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_previousOrganisationId);
        }

        [Fact]
        public void AssignsTheNewOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[2].UnwrapBody<BodyAssignedToOrganisation>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_newOrganisationId);
        }

        public WhenUpdatingBodyOrganisationWithDifferentOrganisation(ITestOutputHelper helper) : base(helper) { }
    }
}
