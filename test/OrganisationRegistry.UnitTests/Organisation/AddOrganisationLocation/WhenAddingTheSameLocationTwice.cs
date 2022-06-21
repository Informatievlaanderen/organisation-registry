namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLocation;

using System;
using System.Collections.Generic;
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
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingTheSameLocationTwice : Specification<AddOrganisationLocationCommandHandler,
        AddOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly bool _isMainLocation;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _ovoNumber;

    public WhenAddingTheSameLocationTwice(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _locationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _ovoNumber = "OVO000012345";
    }

    protected override AddOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationLocationCommandHandler>>().Object,
            session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub()
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
                Guid.NewGuid(),
                _locationId,
                "Gebouw A",
                _isMainLocation,
                null,
                "Location Type A",
                _validFrom,
                _validTo),
        };


    private AddOrganisationLocation AddOrganisationLocationCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
        => await Given(Events)
            .When(AddOrganisationLocationCommand, User)
            .ThenItPublishesTheCorrectNumberOfEvents(0);

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events)
            .When(AddOrganisationLocationCommand, User)
            .ThenThrows<LocationAlreadyCoupledToInThisPeriod>()
            .WithMessage("Deze locatie is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
