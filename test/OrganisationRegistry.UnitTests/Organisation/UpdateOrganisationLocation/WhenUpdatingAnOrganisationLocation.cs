namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Location;
using OrganisationRegistry.Infrastructure.Events;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationLocation :
    Specification<UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly Guid _organisationLocationId;
    private readonly bool _isMainLocation;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _ovoNumber;

    public WhenUpdatingAnOrganisationLocation(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumber = "OVO000012345";
        _organisationId = Guid.NewGuid();

        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            session,
            new DateTimeProvider()
        );

    private IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(_ovoNumber).Build();

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
        };

    private UpdateOrganisationLocation UpdateOrganisationLocationCommand
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo),
            LocationSource.Wegwijs);

    [Fact]
    public async Task PublishesEvent()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesTheOrganisationLocation()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLocationUpdated>>();

        var organisationLocationAdded = PublishedEvents.First().UnwrapBody<OrganisationLocationUpdated>();
        organisationLocationAdded.OrganisationId.Should().Be(_organisationId);
        organisationLocationAdded.LocationId.Should().Be(_locationId);
        organisationLocationAdded.IsMainLocation.Should().Be(_isMainLocation);
        organisationLocationAdded.ValidFrom.Should().Be(_validFrom);
        organisationLocationAdded.ValidTo.Should().Be(_validTo);
    }
}
