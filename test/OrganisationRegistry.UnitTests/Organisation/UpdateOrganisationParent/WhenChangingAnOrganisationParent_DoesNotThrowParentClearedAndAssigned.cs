namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenChangingAnOrganisationParent_DoesNotThrowParentClearedAndAssigned
        : Specification<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
    {
        private OrganisationId _organisationAId;
        private OrganisationId _organisationBId;
        private OrganisationId _organisationCId;
        private Guid _organisationOrganisationParentId;
        private const string OvoNumber = "OVO000012345";

        public WhenChangingAnOrganisationParent_DoesNotThrowParentClearedAndAssigned(ITestOutputHelper helper) : base(
            helper)
        {
        }

        protected override UpdateOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
                Session,
                new DateTimeProvider());

        protected override IUser User
            => new UserBuilder()
                .AddOrganisations(OvoNumber)
                .AddRoles(Role.DecentraalBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationOrganisationParentId = Guid.NewGuid();
            _organisationAId = new OrganisationId(Guid.NewGuid());
            _organisationBId = new OrganisationId(Guid.NewGuid());
            _organisationCId = new OrganisationId(Guid.NewGuid());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationAId,
                    "Kind en Gezin",
                    OvoNumber,
                    "K&G",
                    Article.None,
                    "Kindjes en gezinnetjes",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new OrganisationCreated(
                    _organisationBId,
                    "Ouder en Gezin",
                    "OVO000012346",
                    "O&G",
                    Article.None,
                    "Moeder",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new OrganisationCreated(
                    _organisationCId,
                    "Grootouder en gezin",
                    "OVO000012347",
                    "K&G",
                    Article.None,
                    "Oma",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new OrganisationParentAdded(
                    _organisationAId,
                    _organisationOrganisationParentId,
                    _organisationBId,
                    "Ouder en Gezin",
                    null,
                    null),
                new ParentAssignedToOrganisation(_organisationAId, _organisationBId, _organisationOrganisationParentId),
            };
        }

        protected override UpdateOrganisationParent When()
            => new(
                _organisationOrganisationParentId,
                _organisationAId,
                _organisationCId,
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents
            => 1;

        [Fact]
        public void UpdatesTheOrganisationParent()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

            var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
            organisationParentUpdated.OrganisationId.Should().Be((Guid)_organisationAId);
            organisationParentUpdated.PreviousParentOrganisationId.Should().Be(_organisationBId);
            organisationParentUpdated.ParentOrganisationId.Should().Be((Guid)_organisationCId);
            organisationParentUpdated.ValidFrom.Should().Be(null);
            organisationParentUpdated.ValidTo.Should().Be(null);
        }
    }
}
