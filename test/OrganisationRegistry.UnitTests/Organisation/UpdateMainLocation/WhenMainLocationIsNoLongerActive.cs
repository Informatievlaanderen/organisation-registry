namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainLocation;

using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Xunit.Abstractions;

public class WhenMainLocationIsNoLongerActive : OldSpecification2<UpdateMainLocationCommandHandler, UpdateMainLocation>
{
    private Guid _organisationId;
    private Guid _locationId;
    private Guid _organisationLocationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    public WhenMainLocationIsNoLongerActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateMainLocationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateMainLocationCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

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
    {
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

        return new UpdateMainLocation(new OrganisationId(_organisationId));
    }

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void ClearsTheMainLocation()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
    }
}
