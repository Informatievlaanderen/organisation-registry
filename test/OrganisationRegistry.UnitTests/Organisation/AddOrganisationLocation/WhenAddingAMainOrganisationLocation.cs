namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLocation;

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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAMainOrganisationLocation : OldSpecification2<AddOrganisationLocationCommandHandler,
        AddOrganisationLocation>
{
    private Guid _organisationId;
    private Guid _locationId;
    private Guid _organisationLocationId;
    private bool _isMainLocation;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly string _ovoNumber = "OVO000012345";

    public WhenAddingAMainOrganisationLocation(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationLocationCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub,
            new OrganisationRegistryConfigurationStub());

    protected override IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(_ovoNumber).Build();

    protected override IEnumerable<IEvent> Given()
    {
        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);

        _organisationId = Guid.NewGuid();
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
                "Belgie")
        };
    }

    protected override AddOrganisationLocation When()
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void AddsAnOrganisationLocation()
    {
        var organisationLocationAdded = PublishedEvents.First().UnwrapBody<OrganisationLocationAdded>();
        organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
        organisationLocationAdded.LocationId.Should().Be(_locationId);
        organisationLocationAdded.IsMainLocation.Should().Be(_isMainLocation);
        organisationLocationAdded.ValidFrom.Should().Be(_validFrom);
        organisationLocationAdded.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public void AssignsAMainLocation()
    {
        var organisationLocationAdded = PublishedEvents[1].UnwrapBody<MainLocationAssignedToOrganisation>();
        organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
        organisationLocationAdded.MainLocationId.Should().Be(_locationId);
    }
}
