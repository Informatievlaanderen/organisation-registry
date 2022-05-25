namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainLocation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Xunit.Abstractions;

public class WhenAnotherMainLocationIsNowActive :
    Specification<UpdateMainLocationCommandHandler, UpdateMainLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationAId;
    private readonly Guid _locationBId;
    private readonly Guid _organisationLocationAId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public WhenAnotherMainLocationIsNowActive(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _locationAId = Guid.NewGuid();
        _locationBId = Guid.NewGuid();

        _organisationLocationAId = Guid.NewGuid();
        _dateTimeProviderStub.StubDate = _dateTimeProviderStub.StubDate.AddDays(1);
    }

    protected override UpdateMainLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateMainLocationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IEvent[] Events
        => new IEvent[]
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

    private UpdateMainLocation UpdateMainLocationCommand
        => new(new OrganisationId(_organisationId));

   [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events).When(UpdateMainLocationCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task ClearsTheMainLocation()
    {
        await Given(Events).When(UpdateMainLocationCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(2);
        PublishedEvents[0].Should().BeOfType<Envelope<MainLocationClearedFromOrganisation>>();
    }

    [Fact]
    public async Task AssignsTheNewLocation()
    {
        await Given(Events).When(UpdateMainLocationCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(2);

        var mainLocationAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<MainLocationAssignedToOrganisation>();
        mainLocationAssignedToOrganisation.Should().NotBeNull();
        mainLocationAssignedToOrganisation.MainLocationId.Should().Be(_locationBId);
    }
}
