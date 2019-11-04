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

    public class WhenAddingAParentWithOverlappingValidity: ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private Guid _organisationId;
        private Guid _organisationOrganisationParentId1;
        private Guid _organisationOrganisationParentId2;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProviderStub;
        private Guid _organisationParentId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,

                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>());
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

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new OrganisationCreated(_organisationParentId, "Ouder en Gezin", "OVO000012346", "O&G", "Moeder", new List<Purpose>(),false, null, null),
                new OrganisationParentAdded(_organisationId, _organisationOrganisationParentId1, _organisationParentId, "Ouder en Gezin", null, null)
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                _organisationOrganisationParentId2,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriodException>();
            Exception.Message.Should().Be("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.");
        }

        public WhenAddingAParentWithOverlappingValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
