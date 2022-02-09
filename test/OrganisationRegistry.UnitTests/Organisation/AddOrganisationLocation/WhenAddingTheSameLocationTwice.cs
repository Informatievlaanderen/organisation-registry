namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLocation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Location;
    using OrganisationRegistry.Infrastructure.Events;
    using Location.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingTheSameLocationTwice: ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationLocation>
    {
        private Guid _organisationId;
        private Guid _locationId;
        private Guid _organisationLocationId;
        private bool _isMainLocation;
        private DateTime _validTo;
        private DateTime _validFrom;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                new OrganisationRegistryConfigurationStub(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();
            _locationId = Guid.NewGuid();
            _organisationLocationId = Guid.NewGuid();
            _isMainLocation = true;
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new LocationCreated(_locationId, "12345", "Albert 1 laan 32, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationId, _organisationLocationId, _locationId, "Gebouw A", _isMainLocation, null, null, _validFrom, _validTo)
            };
        }

        protected override AddOrganisationLocation When()
        {
            return new AddOrganisationLocation(
                Guid.NewGuid(),
                new OrganisationId(_organisationId),
                new LocationId(_locationId),
                _isMainLocation,
                null,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<LocationAlreadyCoupledToInThisPeriod>();
            Exception.Message.Should().Be("Deze locatie is in deze periode reeds gekoppeld aan de organisatie.");
        }

        public WhenAddingTheSameLocationTwice(ITestOutputHelper helper) : base(helper) { }
    }
}
