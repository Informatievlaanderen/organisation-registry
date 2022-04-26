namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationParent : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationParent>
    {
        private OrganisationId _organisationId;
        private OrganisationId _organisationParentId;
        private Guid _organisationOrganisationParentId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProviderStub;
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

            _organisationOrganisationParentId = Guid.NewGuid();
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);
            _organisationId = new OrganisationId(Guid.NewGuid());
            _organisationParentId = new OrganisationId(Guid.NewGuid());
            _ovoNumber = "OVO000012345";

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", _ovoNumber, "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", Article.None, "Moeder", new List<Purpose>(), false, null, null, null, null),
                new OrganisationParentAdded(_organisationId, _organisationOrganisationParentId, _organisationParentId, "Ouder en Gezin", null, null),
                new ParentAssignedToOrganisation(_organisationId, _organisationParentId, _organisationOrganisationParentId)
            };
        }

        protected override UpdateOrganisationParent When()
        {
            return new UpdateOrganisationParent(
                _organisationOrganisationParentId,
                _organisationId,
                _organisationParentId,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder()
                    .AddOrganisations(_ovoNumber)
                    .AddRoles(Role.DecentraalBeheerder)
                    .Build()
            };;
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void UpdatesTheOrganisationParent()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

            var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
            organisationParentUpdated.OrganisationId.Should().Be((Guid)_organisationId);
            organisationParentUpdated.ParentOrganisationId.Should().Be((Guid)_organisationParentId);
            organisationParentUpdated.ValidFrom.Should().Be(_validFrom);
            organisationParentUpdated.ValidTo.Should().Be(_validTo);
        }

        public WhenUpdatingAnOrganisationParent(ITestOutputHelper helper) : base(helper) { }
    }
}
