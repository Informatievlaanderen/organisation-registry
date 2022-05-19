namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Location;
using OrganisationRegistry.Infrastructure.Events;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenValidityBecomesInvalidAndIsMainLocationChangesToFalseBugfix : Specification<
    UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private Guid _organisationId;
    private Guid _locationId;
    private Guid _organisationLocationId;
    private DateTime? _validTo;
    private DateTime _validFrom;
    private const string OvoNumber = "OVO000012345";

    public WhenValidityBecomesInvalidAndIsMainLocationChangesToFalseBugfix(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            Session,
            new DateTimeProviderStub(new DateTime(2017, 01, 19)));

    protected override IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(OvoNumber).Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _validFrom = new DateTime(1980, 10, 17);
        _validTo = null;

        return new List<IEvent>
        {
            new OrganisationCreated(
                _organisationId,
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
                "Location Type A",
                _validFrom,
                _validTo),
            new MainLocationAssignedToOrganisation(_organisationId, _locationId, _organisationLocationId)
        };
    }

    protected override UpdateOrganisationLocation When()
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            false,
            null,
            new ValidFrom(new DateTime(1980, 10, 17)),
            new ValidTo(new DateTime(2016, 06, 16)),
            Source.Wegwijs);

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationLocation()
    {
        var @event = PublishedEvents[0];
        @event.Should().BeOfType<Envelope<OrganisationLocationUpdated>>();

        var organisationLocationAdded = PublishedEvents.First().UnwrapBody<OrganisationLocationUpdated>();
        organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
        organisationLocationAdded.LocationId.Should().Be(_locationId);
        organisationLocationAdded.IsMainLocation.Should().Be(false);
        organisationLocationAdded.ValidFrom.Should().Be(new DateTime(1980, 10, 17));
        organisationLocationAdded.ValidTo.Should().Be(new DateTime(2016, 06, 16));
    }

    [Fact]
    public void ClearsTheMainLocation()
    {
        var @event = PublishedEvents[1];
        @event.Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();

        var mainLocationClearedFromOrganisation = @event.UnwrapBody<MainLocationClearedFromOrganisation>();
        mainLocationClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        mainLocationClearedFromOrganisation.MainLocationId.Should().Be(_locationId);
    }
}
