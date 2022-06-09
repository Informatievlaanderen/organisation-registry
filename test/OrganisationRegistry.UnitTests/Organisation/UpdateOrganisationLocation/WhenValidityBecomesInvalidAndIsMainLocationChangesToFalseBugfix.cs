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

public class WhenValidityBecomesInvalidAndIsMainLocationChangesToFalseBugfix :
    Specification<UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly Guid _organisationLocationId;
    private readonly DateTime? _validTo;
    private readonly DateTime _validFrom;
    private readonly string _ovoNumber;

    public WhenValidityBecomesInvalidAndIsMainLocationChangesToFalseBugfix(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumber = "OVO000012345";
        _organisationId = Guid.NewGuid();

        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _validFrom = new DateTime(1980, 10, 17);
        _validTo = null;
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(new DateTime(2017, 01, 19)));

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
                true,
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
            false,
            null,
            new ValidFrom(new DateTime(1980, 10, 17)),
            new ValidTo(new DateTime(2016, 06, 16)),
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
    public async Task ClearsTheMainLocation()
    {
        await Given(Events).When(UpdateOrganisationLocationCommand, User).Then();
        var @event = PublishedEvents[1];
#pragma warning disable CS0618
        @event.Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
#pragma warning restore CS0618

#pragma warning disable CS0618
        var mainLocationClearedFromOrganisation = @event.UnwrapBody<MainLocationClearedFromOrganisation>();
#pragma warning restore CS0618
        mainLocationClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        mainLocationClearedFromOrganisation.MainLocationId.Should().Be(_locationId);
    }
}
