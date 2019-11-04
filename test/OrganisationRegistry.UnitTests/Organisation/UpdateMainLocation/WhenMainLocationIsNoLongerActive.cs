namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainLocation
{
    using System;
    using System.Collections.Generic;
    using Xunit;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Location.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using Xunit.Abstractions;

    public class WhenMainLocationIsNoLongerActive : Specification<Organisation, OrganisationCommandHandlers, UpdateMainLocation>
    {
        private Guid _organisationId;
        private Guid _locationId;
        private Guid _organisationLocationId;
        private DateTimeProviderStub _dateTimeProviderStub;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

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
            _organisationId = Guid.NewGuid();
            _locationId = Guid.NewGuid();
            _organisationLocationId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new LocationCreated(_locationId, "12345", "Albert 1 laan 32, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationId, _organisationLocationId, _locationId, "Gebouw A", true, null, null, DateTime.Today, DateTime.Today),
                new MainLocationAssignedToOrganisation(_organisationId, _locationId, _organisationLocationId)
            };
        }

        protected override UpdateMainLocation When()
        {
            _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

            return new UpdateMainLocation(new OrganisationId(_organisationId));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void ClearsTheMainLocation()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
        }

        public WhenMainLocationIsNoLongerActive(ITestOutputHelper helper) : base(helper) { }
    }
}
