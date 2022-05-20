namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainLocation;

using System;
using System.Collections.Generic;
using Location.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit.Abstractions;

public class WhenMainLocationIsStillActive : OldSpecification2<UpdateMainLocationCommandHandler, UpdateMainLocation>
{
    private Guid _organisationId;
    private Guid _locationId;
    private Guid _organisationLocationId;

    public WhenMainLocationIsStillActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateMainLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateMainLocationCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder().Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();
        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();

        return new List<IEvent>
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
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
                null,
                DateTime.Today,
                DateTime.Today),
            new MainLocationAssignedToOrganisation(_organisationId, _locationId, _organisationLocationId)
        };
    }

    protected override UpdateMainLocation When()
        => new(new OrganisationId(_organisationId));

    protected override int ExpectedNumberOfEvents
        => 0;
}
