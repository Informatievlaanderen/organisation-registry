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

public class WhenAnotherMainLocationIsNowActive :
    OldSpecification2<UpdateMainLocationCommandHandler, UpdateMainLocation>
{
    private Guid _organisationId;
    private Guid _locationAId;
    private Guid _locationBId;
    private Guid _organisationLocationAId;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    public WhenAnotherMainLocationIsNowActive(ITestOutputHelper helper) : base(helper)
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
        _locationAId = Guid.NewGuid();
        _locationBId = Guid.NewGuid();

        _organisationLocationAId = Guid.NewGuid();
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
                _locationAId,
                "12345",
                "Albert 1 laan 32, 1000 Brussel",
                "Albert 1 laan 32",
                "1000",
                "Brussel",
                "Belgie"),
            new LocationCreated(
                _locationBId,
                "12345",
                "Boudewijn 1 laan 32, 1000 Brussel",
                "Boudewijn 1 laan 32",
                "1000",
                "Brussel",
                "Belgie"),
            new OrganisationLocationAdded(
                _organisationId,
                _organisationLocationAId,
                _locationAId,
                "Gebouw A",
                true,
                null,
                null,
                DateTime.Today,
                DateTime.Today),
            new MainLocationAssignedToOrganisation(_organisationId, _locationAId, _organisationLocationAId),
            new OrganisationLocationAdded(
                _organisationId,
                Guid.NewGuid(),
                _locationBId,
                "Gebouw B",
                true,
                null,
                null,
                DateTime.Today.AddDays(1),
                DateTime.Today.AddDays(1))
        };
    }

    protected override UpdateMainLocation When()
    {
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);

        return new UpdateMainLocation(new OrganisationId(_organisationId));
    }

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void ClearsTheMainLocation()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
    }

    [Fact]
    public void AssignsTheNewLocation()
    {
        var mainLocationAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<MainLocationAssignedToOrganisation>();
        mainLocationAssignedToOrganisation.Should().NotBeNull();
        mainLocationAssignedToOrganisation.MainLocationId.Should().Be(_locationBId);
    }
}
