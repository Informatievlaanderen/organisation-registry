namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Location.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Location;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenUpdatingAnActiveMainOrganisationLocationToAnInactiveOne :
        Specification<UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly Guid _organisationLocationId;
    private readonly bool _isMainLocation;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProvider;
    private readonly string _ovoNumber;

    public WhenUpdatingAnActiveMainOrganisationLocationToAnInactiveOne(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProvider = new DateTimeProviderStub(DateTime.Today);
        _ovoNumber = "OVO000012345";
        _organisationId = Guid.NewGuid();
        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = _dateTimeProvider.Today;
        _validTo = _dateTimeProvider.Today;
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            session,
            _dateTimeProvider
        );

    private IUser User
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_ovoNumber)
            .Build();

    private IEvent[] Events
        => new IEvent[]
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
                _isMainLocation,
                null,
                "Location Type A",
                _validFrom,
                _validTo),
#pragma warning disable CS0618
            new MainLocationAssignedToOrganisation(_organisationId, _locationId, _organisationLocationId)
#pragma warning restore CS0618
        };

    private UpdateOrganisationLocation UpdateOrganisationLocationCommand
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom.AddYears(1)),
            new ValidTo(_validTo.AddYears(1)),
            LocationSource.Wegwijs);

    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationLocation()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationLocationUpdated>>();

        var organisationLocationUpdated = PublishedEvents.First().UnwrapBody<OrganisationLocationUpdated>();
        organisationLocationUpdated.OrganisationId.Should().Be(_organisationId);
        organisationLocationUpdated.LocationId.Should().Be(_locationId);
        organisationLocationUpdated.IsMainLocation.Should().Be(_isMainLocation);
        organisationLocationUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
        organisationLocationUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
    }

    [Fact]
    public async Task ClearsTheMainLocation()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).Then();
#pragma warning disable CS0618
        PublishedEvents[1].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
#pragma warning restore CS0618

#pragma warning disable CS0618
        var organisationLocationUpdated = PublishedEvents[1].UnwrapBody<MainLocationClearedFromOrganisation>();
#pragma warning restore CS0618
        organisationLocationUpdated.OrganisationId.Should().Be(_organisationId);
        organisationLocationUpdated.MainLocationId.Should().Be(_locationId);
    }
}
