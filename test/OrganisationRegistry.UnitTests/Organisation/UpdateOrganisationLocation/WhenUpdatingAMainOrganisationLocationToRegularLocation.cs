namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Location;
    using OrganisationRegistry.Infrastructure.Events;
    using Location.Events;
    using LocationType;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Locations;
    using Xunit;
    using Xunit.Abstractions;

    public class
        WhenUpdatingAMainOrganisationLocationToRegularLocation : Specification<UpdateOrganisationLocationCommandHandler,
            UpdateOrganisationLocation>
    {
        private Guid _organisationId;
        private Guid _locationId;
        private Guid _organisationLocationId;
        private bool _isMainLocation;
        private DateTime _validTo;
        private DateTime _validFrom;
        private readonly DateTimeProviderStub _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        private string _ovoNumber = "OVO000012345";

        public WhenUpdatingAMainOrganisationLocationToRegularLocation(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override UpdateOrganisationLocationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub
            );

        protected override IUser User
            => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(_ovoNumber).Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _locationId = Guid.NewGuid();
            _organisationLocationId = Guid.NewGuid();
            _isMainLocation = false;
            _validFrom = _dateTimeProviderStub.Today.AddDays(0);
            _validTo = _dateTimeProviderStub.Today.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "Kind en Gezin",
                    _ovoNumber,
                    "K&G",
                    Article.None,
                    "Kindjes en gezinnetjes",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new LocationCreated(
                    _locationId,
                    "12345",
                    "Albert 1 laan 32, 1000 Brussel",
                    "Albert 1 laan 32",
                    "1000",
                    "Brussel",
                    "Belgie"),
                new OrganisationLocationAdded(
                    _organisationId,
                    _organisationLocationId,
                    _locationId,
                    "Gebouw A",
                    true,
                    null,
                    "Loaction Type A",
                    _validFrom,
                    _validTo),
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
                new ValidFrom(_validFrom),
                new ValidTo(_validTo),
                Source.Wegwijs);
        }

        protected override int ExpectedNumberOfEvents
            => 2;

        [Fact]
        public void UpdatesTheOrganisationLocation()
        {
            PublishedEvents[0].Should().BeOfType<Envelope<OrganisationLocationUpdated>>();

            var organisationLocationAdded = PublishedEvents.First().UnwrapBody<OrganisationLocationUpdated>();
            organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
            organisationLocationAdded.LocationId.Should().Be(_locationId);
            organisationLocationAdded.IsMainLocation.Should().Be(_isMainLocation);
            organisationLocationAdded.ValidFrom.Should().Be(_validFrom);
            organisationLocationAdded.ValidTo.Should().Be(_validTo);
        }

        [Fact]
        public void ClearsTheMainLocation()
        {
            var mainLocationAssignedToOrganisation =
                PublishedEvents[1].UnwrapBody<MainLocationClearedFromOrganisation>();
            mainLocationAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
            mainLocationAssignedToOrganisation.MainLocationId.Should().Be(_locationId);
        }
    }
}
