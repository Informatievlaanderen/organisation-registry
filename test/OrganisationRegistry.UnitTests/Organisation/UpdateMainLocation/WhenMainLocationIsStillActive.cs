namespace OrganisationRegistry.UnitTests.Organisation.UpdateMainLocation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Location.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenMainLocationIsStillActive : Specification<UpdateMainLocationCommandHandler, UpdateMainLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly Guid _organisationLocationId;

    public WhenMainLocationIsStillActive(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
    }

    protected override UpdateMainLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateMainLocationCommandHandler>>().Object,
            session,
            new DateTimeProvider());

    private IEvent[] Events
        => new IEvent[] {
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

    private UpdateMainLocation UpdateMainLocationCommand
        => new(new OrganisationId(_organisationId));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateMainLocationCommand, UserBuilder.User()).ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
