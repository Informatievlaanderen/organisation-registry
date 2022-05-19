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
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationLocationToAnAlreadyCoupledLocation : ExceptionSpecification<
    UpdateOrganisationLocationCommandHandler, UpdateOrganisationLocation>
{
    private Guid _organisationId;
    private Guid _locationAId;
    private Guid _locationBId;
    private const string OvoNumber = "OVO000012345";
    private Guid _organisationLocationBId;
    private Guid _organisationLocationAId;

    public WhenUpdatingAnOrganisationLocationToAnAlreadyCoupledLocation(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationLocationCommandHandler>>().Object,
            Session,
            new DateTimeProvider()
        );

    protected override IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(OvoNumber).Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();
        _locationAId = Guid.NewGuid();
        _locationBId = Guid.NewGuid();
        _organisationLocationAId = Guid.NewGuid();
        _organisationLocationBId = Guid.NewGuid();

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
                _organisationLocationAId,
                _locationAId,
                "Gebouw A",
                false,
                null,
                "Location Type A",
                null,
                null) { Version = 2 },
            new OrganisationLocationAdded(
                _organisationId,
                _organisationLocationBId,
                _locationBId,
                "Gebouw B",
                false,
                null,
                "Location Type A",
                null,
                null) { Version = 3 }
        };
    }

    protected override UpdateOrganisationLocation When()
        => new(
            _organisationLocationBId,
            new OrganisationId(_organisationId),
            new LocationId(_locationAId),
            false,
            null,
            new ValidFrom(),
            new ValidTo(),
            Source.Wegwijs);

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<LocationAlreadyCoupledToInThisPeriod>();
        Exception?.Message.Should().Be("Deze locatie is in deze periode reeds gekoppeld aan de organisatie.");
    }
}
