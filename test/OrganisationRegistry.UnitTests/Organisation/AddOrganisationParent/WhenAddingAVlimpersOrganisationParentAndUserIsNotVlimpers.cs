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
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAVlimpersOrganisationParentAndUserIsNotVlimpers : ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
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
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
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
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new OrganisationPlacedUnderVlimpersManagement(_organisationId),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", Article.None, "Moeder", new List<Purpose>(), false, null, null, null, null)
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                _organisationOrganisationParentId,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder().Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsException()
        {
            Exception.Should().BeOfType<UserIsNotAuthorizedForVlimpersOrganisations>();
        }

        public WhenAddingAVlimpersOrganisationParentAndUserIsNotVlimpers(ITestOutputHelper helper) : base(helper) { }
    }
}
