namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging.Abstractions;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganisationLocation : Specification<DeleteOrganisationLocationCommandHandler, DeleteOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationLocationId;

    public WhenRemovingAnOrganisationLocation(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
    }

    protected override DeleteOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(NullLogger<DeleteOrganisationLocationCommandHandler>.Instance, session);

    private DeleteOrganisationLocation DeleteOrganisationLocation
        => new(new OrganisationId(_organisationId), _organisationLocationId);

    [Fact]
    public async Task AnOrganisationLocationRemovedEventIsPublished()
    {
        await Given(OrganisationCreated, OrganisationLocationAdded)
            .When(DeleteOrganisationLocation, TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLocationRemoved>>();
        var organisationLocationRemoved = PublishedEvents.First().UnwrapBody<OrganisationLocationRemoved>();
        organisationLocationRemoved.OrganisationId.Should().Be(_organisationId);
        organisationLocationRemoved.OrganisationLocationId.Should().Be(_organisationLocationId);
    }

    private OrganisationCreated OrganisationCreated
        => new(
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
            null);

    private OrganisationLocationAdded OrganisationLocationAdded
        => new(
            _organisationId,
            _organisationLocationId,
            Guid.NewGuid(),
            "",
            false,
            null,
            null,
            null,
            null);
}
