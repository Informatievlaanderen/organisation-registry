namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent
{
    using System;
    using System.Collections.Generic;
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

    public class WhenAddingAnOrganisationParent : Specification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private Guid _organisationId;
        private Guid _organisationOrganisationParentId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProviderStub;
        private Guid _organisationParentId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            _organisationOrganisationParentId = Guid.NewGuid();
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);
            _organisationId = Guid.NewGuid();
            _organisationParentId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", "Moeder", new List<Purpose>(),false, null, null)
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                _organisationOrganisationParentId,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void AddsAnOrganisationParent()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

            organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId);
            organisationParentAdded.OrganisationId.Should().Be(_organisationId);
            organisationParentAdded.ParentOrganisationId.Should().Be(_organisationParentId);
            organisationParentAdded.ValidFrom.Should().Be(_validFrom);
            organisationParentAdded.ValidTo.Should().Be(_validTo);
        }

        [Fact]
        public void AssignsAParent()
        {
            var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<ParentAssignedToOrganisation>();
            parentAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
            parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_organisationParentId);
        }

        public WhenAddingAnOrganisationParent(ITestOutputHelper helper) : base(helper) { }
    }
}
