namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation;

using System;
using System.Collections.Generic;
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
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenMakingAnOrganisationLocationAMainLocationWhenThereAlreadyIsOne : ExceptionOldSpecification2<
    UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private Guid _organisationId;
    private Guid _organisationLocationId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private Guid _locationAId;
    private Guid _locationBId;
    private const string OvoNumber = "OVO000012345";

    public WhenMakingAnOrganisationLocationAMainLocationWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(
        helper)
    {
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(OvoNumber).Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _organisationLocationId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _locationAId = Guid.NewGuid();
        _locationBId = Guid.NewGuid();

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
                Guid.NewGuid(),
                _locationAId,
                "Gebouw A",
                true,
                null,
                "Location Type A",
                _validFrom,
                _validTo),
            new OrganisationLocationAdded(
                _organisationId,
                _organisationLocationId,
                _locationBId,
                "Gebouw B",
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
            new LocationId(_locationBId),
            true,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo),
            Source.Wegwijs);

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<OrganisationAlreadyHasAMainLocationInThisPeriod>();
        Exception?.Message.Should().Be("Deze organisatie heeft reeds een hoofdlocatie binnen deze periode.");
    }
}
