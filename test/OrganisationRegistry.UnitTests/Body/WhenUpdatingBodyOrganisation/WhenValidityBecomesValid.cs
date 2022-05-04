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

    public class WhenValidityBecomesValid : OldSpecification<Body, BodyCommandHandlers, UpdateBodyOrganisation>
    {
        private Guid _bodyId;
        private Guid _bodyOrganisationId;
        private Guid _organisationId;
        private DateTimeProviderStub _dateTimeProviderStub;
        private DateTime _yesterday;

        protected override BodyCommandHandlers BuildHandler()
        {
            return new BodyCommandHandlers(
                new Mock<ILogger<BodyCommandHandlers>>().Object,
                Session,
                _dateTimeProviderStub,
                new SequentialBodyNumberGenerator(),
                Mock.Of<IUniqueBodyNumberValidator>(),
                Mock.Of<IBodySeatNumberGenerator>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
            _yesterday = _dateTimeProviderStub.Today.AddDays(-1);
            _bodyId = Guid.NewGuid();
            _organisationId = Guid.NewGuid();
            _bodyOrganisationId = Guid.NewGuid();
            return new List<IEvent>
            {
                new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
                new OrganisationCreated(_organisationId, "orgName", "ovoNumber", "shortName", string.Empty, "description",
                    new List<Purpose>(), false, null, null, null, null),
                new BodyOrganisationAdded(_bodyId, _bodyOrganisationId, "bodyName", _organisationId, "orgName",
                    _yesterday, _yesterday),
            };
        }

        protected override UpdateBodyOrganisation When()
        {
            return new UpdateBodyOrganisation(
                new BodyId(_bodyId),
                new BodyOrganisationId(_bodyOrganisationId),
                new OrganisationId(_organisationId),
                new Period());
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
        public void AssignsTheBodyOrganisation()
        {
            var bodyBalancedParticipationChanged = PublishedEvents[1].UnwrapBody<BodyAssignedToOrganisation>();
            bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

            bodyBalancedParticipationChanged.OrganisationId.Should().Be(_organisationId);
        }

        public WhenValidityBecomesValid(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
