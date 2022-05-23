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
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenUpdatingAnOrganisationLocationToMainLocation : OldSpecification2<UpdateOrganisationLocationCommandHandler,
        UpdateOrganisationLocation>
{
    private Guid _organisationId;
    private Guid _locationId;
    private Guid _organisationLocationId;
    private bool _isMainLocation;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
    private const string OvoNumber = "OVO000012345";

    public WhenUpdatingAnOrganisationLocationToMainLocation(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub
        );

    protected override IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(OvoNumber).Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = _dateTimeProviderStub.Today.AddDays(0);
        _validTo = _dateTimeProviderStub.Today.AddDays(2);

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
                false,
                null,
                "Location Type A",
                _validFrom,
                _validTo)
        };
    }

    protected override UpdateOrganisationLocation When()
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo),
            Source.Wegwijs);

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
    public void AssignsTheMainLocation()
    {
        var mainLocationAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<MainLocationAssignedToOrganisation>();
        mainLocationAssignedToOrganisation.OrganisationLocationId.Should().Be(_organisationLocationId);
        mainLocationAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
        mainLocationAssignedToOrganisation.MainLocationId.Should().Be(_locationId);
    }
}
