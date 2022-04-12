namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAParentWithDifferentValidity: Specification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private Guid _organisationId;
        private Guid _organisationOrganisationParentId1;
        private Guid _organisationOrganisationParentId2;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProviderStub;
        private Guid _organisationParentId;
        private string _ovoNumber;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            _organisationOrganisationParentId1 = Guid.NewGuid();
            _organisationOrganisationParentId2 = Guid.NewGuid();
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);
            _organisationId = Guid.NewGuid();
            _organisationParentId = Guid.NewGuid();

            _ovoNumber = "OVO000012345";
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", _ovoNumber, "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", Article.None, "Moeder", new List<Purpose>(), false, null, null, null, null),
                new OrganisationParentAdded(_organisationId, _organisationOrganisationParentId1, _organisationParentId, "Ouder en Gezin", _validFrom, _validTo),
                new ParentAssignedToOrganisation(_organisationId, _organisationParentId, _organisationOrganisationParentId1)
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                _organisationOrganisationParentId2,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom.AddYears(1)),
                new ValidTo(_validTo.AddYears(1)))
            {
                User = new UserBuilder()
                    .AddOrganisations(_ovoNumber)
                    .AddRoles(Role.DecentraalBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void AddsAnOrganisationParent()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

            organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId2);
            organisationParentAdded.OrganisationId.Should().Be(_organisationId);
            organisationParentAdded.ParentOrganisationId.Should().Be(_organisationParentId);
            organisationParentAdded.ValidFrom.Should().Be(_validFrom.AddYears(1));
            organisationParentAdded.ValidTo.Should().Be(_validTo.AddYears(1));
        }

        public WhenAddingAParentWithDifferentValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
