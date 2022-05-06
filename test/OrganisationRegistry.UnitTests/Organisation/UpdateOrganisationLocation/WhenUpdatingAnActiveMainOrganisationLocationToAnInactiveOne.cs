namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Location.Events;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Location;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnActiveMainOrganisationLocationToAnInactiveOne : OldSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationLocation>
    {
        private Guid _organisationId;
        private Guid _locationId;
        private Guid _organisationLocationId;
        private bool _isMainLocation;
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProvider;
        private string _ovoNumber;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                _dateTimeProvider,
                new OrganisationRegistryConfigurationStub(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProvider = new DateTimeProviderStub(DateTime.Today);
            _organisationId = Guid.NewGuid();

            _locationId = Guid.NewGuid();
            _organisationLocationId = Guid.NewGuid();
            _isMainLocation = true;
            _validFrom = _dateTimeProvider.Today;
            _validTo = _dateTimeProvider.Today;

            _ovoNumber = "OVO000012345";
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", _ovoNumber, "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new LocationCreated(_locationId, "12345", "Albert 1 laan 32, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationId, _organisationLocationId, _locationId, "Gebouw A", _isMainLocation, null, null, _validFrom, _validTo),
                new MainLocationAssignedToOrganisation(_organisationId, _locationId, _organisationLocationId)
            };
        }

        protected override UpdateOrganisationLocation When()
        {
            return new UpdateOrganisationLocation(
                _organisationLocationId,
                new OrganisationId(_organisationId),
                new LocationId(_locationId),
                _isMainLocation,
                null,
                new ValidFrom(_validFrom.AddYears(1)),
                new ValidTo(_validTo.AddYears(1)),
                Source.Wegwijs)
            {
                User = new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(_ovoNumber).Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 2;

        [Fact]
        public void UpdatesTheOrganisationLocation()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationLocationUpdated>>();

            var organisationLocationUpdated = PublishedEvents.First().UnwrapBody<OrganisationLocationUpdated>();
            organisationLocationUpdated.OrganisationId.Should().Be(_organisationId);
            organisationLocationUpdated.LocationId.Should().Be(_locationId);
            organisationLocationUpdated.IsMainLocation.Should().Be(_isMainLocation);
            organisationLocationUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
            organisationLocationUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
        }

        [Fact]
        public void ClearsTheMainLocation()
        {
            PublishedEvents[1].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();

            var organisationLocationUpdated = PublishedEvents[1].UnwrapBody<MainLocationClearedFromOrganisation>();
            organisationLocationUpdated.OrganisationId.Should().Be(_organisationId);
            organisationLocationUpdated.MainLocationId.Should().Be(_locationId);
        }

        public WhenUpdatingAnActiveMainOrganisationLocationToAnInactiveOne(ITestOutputHelper helper) : base(helper) { }
    }
}
