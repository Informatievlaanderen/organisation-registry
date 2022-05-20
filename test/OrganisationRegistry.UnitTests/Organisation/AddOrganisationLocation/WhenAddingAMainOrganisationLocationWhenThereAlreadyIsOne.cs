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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAMainOrganisationLocationWhenThereAlreadyIsOne
    : Specification<AddOrganisationLocationCommandHandler, AddOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationAId;
    private readonly Guid _locationBId;
    private readonly Guid _organisationLocationId;
    private readonly bool _isMainLocation;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    private readonly string _ovoNumber;

    public WhenAddingAMainOrganisationLocationWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(
        helper)
    {
        _organisationId = Guid.NewGuid();
        _locationAId = Guid.NewGuid();
        _locationBId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
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
            new OrganisationRegistryConfigurationStub());

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
                _locationAId,
                "12345",
                "Albert 1 laan 32, 1000 Brussel",
                "Albert 1 laan 32",
                "1000",
                "Brussel",
                "Belgie"),
            new LocationCreated(
                _locationBId,
                "12346",
                "Albert 1 laan 34, 1000 Brussel",
                "Albert 1 laan 32",
                "1000",
                "Brussel",
                "Belgie"),
            new OrganisationLocationAdded(
                _organisationId,
                _organisationLocationId,
                _locationAId,
                "Gebouw A",
                _isMainLocation,
                null,
                "Location Type A",
                _validFrom,
                _validTo)
        };

    private AddOrganisationLocation AddOrganisationLocationCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationId),
            new LocationId(_locationBId),
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
            .ThenThrows<OrganisationAlreadyHasAMainLocationInThisPeriod>()
            .WithMessage("Deze organisatie heeft reeds een hoofdlocatie binnen deze periode.");
    }
}
