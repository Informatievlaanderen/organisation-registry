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

    public class WhenValidityBecomesInvalid : Specification<Body, BodyCommandHandlers, UpdateBodyOrganisation>
    {
        private Guid _bodyId;
        private Guid _bodyOrganisationId;
        private Guid _organisationId;

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
            _organisationId = Guid.NewGuid();
            _bodyOrganisationId = Guid.NewGuid();
            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
                new OrganisationCreated(_organisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new BodyOrganisationAdded(_bodyId, _bodyOrganisationId, "bodyName", _organisationId, "orgName",
                    null, null),
                new BodyAssignedToOrganisation(_bodyId, "Body", _organisationId, "orgName", _bodyOrganisationId)
            };
        }

        protected override UpdateBodyOrganisation When()
        {
            return new UpdateBodyOrganisation(
                new BodyId(_bodyId),
                new BodyOrganisationId(_bodyOrganisationId),
                new OrganisationId(_organisationId),
                new Period(new ValidFrom(DateTime.MinValue), new ValidTo(DateTime.MinValue)));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheBodyOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_organisationId);
        }

        [Fact]
        public void ClearsTheBodyOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[1].UnwrapBody<BodyClearedFromOrganisation>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_organisationId);
        }

        public WhenValidityBecomesInvalid(ITestOutputHelper helper) : base(helper) { }
    }
}
