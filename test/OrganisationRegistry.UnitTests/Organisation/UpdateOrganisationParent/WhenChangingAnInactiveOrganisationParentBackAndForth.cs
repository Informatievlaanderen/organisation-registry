namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    // This test was introduced to fix an issue where the previous organisationid was not correctly set when updating the parent back and forth.
    // see the commit of this comment to see how it was fixed.
    public class WhenChangingAnInactiveOrganisationParentBackAndForth : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationParent>
    {
        private Guid _organisationId;
        private Guid _organisationParentId;
        private Guid _organisationGrandParentId;
        private Guid _organisationOrganisationParentId;
        private DateTime _validTo;
        private DateTime _validFrom;

        private static readonly DateTime Yesterday = DateTime.Now.AddDays(-1);

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationOrganisationParentId = Guid.NewGuid();
            _validFrom = Yesterday;
            _validTo = Yesterday;
            _organisationId = Guid.NewGuid();
            _organisationParentId = Guid.NewGuid();
            _organisationGrandParentId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, Yesterday, Yesterday),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", "Moeder", new List<Purpose>(),false, Yesterday, Yesterday),
                new OrganisationCreated(_organisationGrandParentId, "Grootouder en gezin", "OVO000012347", "K&G", "Oma", new List<Purpose>(),false, Yesterday, Yesterday),
                new OrganisationParentAdded(_organisationId, _organisationOrganisationParentId, _organisationParentId, "Ouder en Gezin", Yesterday, Yesterday),
                new OrganisationParentUpdated(
                    _organisationId,
                    _organisationOrganisationParentId,
                    _organisationGrandParentId, "", Yesterday, Yesterday,
                    _organisationParentId, "", null, null)
            };
        }

        protected override UpdateOrganisationParent When()
        {
            return new UpdateOrganisationParent(
                _organisationOrganisationParentId,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void UpdatesTheOrganisationParent()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

            var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
            organisationParentUpdated.OrganisationId.Should().Be(_organisationId);
            organisationParentUpdated.PreviousParentOrganisationId.Should().Be(_organisationGrandParentId);
            organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationParentId);
            organisationParentUpdated.ValidFrom.Should().Be(_validFrom);
            organisationParentUpdated.ValidTo.Should().Be(_validTo);
        }

        public WhenChangingAnInactiveOrganisationParentBackAndForth(ITestOutputHelper helper) : base(helper) { }
    }
}
