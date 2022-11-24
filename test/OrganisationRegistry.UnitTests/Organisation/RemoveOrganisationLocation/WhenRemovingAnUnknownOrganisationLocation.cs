namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging.Abstractions;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnUnknownOrganisationLocation : Specification<DeleteOrganisationLocationCommandHandler, DeleteOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationLocationId;
    private readonly Guid _otherOrganisationLocationId;

    public WhenRemovingAnUnknownOrganisationLocation(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _otherOrganisationLocationId = Guid.NewGuid();
    }

    protected override DeleteOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(NullLogger<DeleteOrganisationLocationCommandHandler>.Instance, session);

    private DeleteOrganisationLocation DeleteOrganisationLocation
        => new(new OrganisationId(_organisationId), _otherOrganisationLocationId);

    [Fact]
    public async Task ThrowsAnOrganistionLocationNotFoundException()
    {
        await Given(OrganisationCreated, OrganisationLocationAdded)
            .When(DeleteOrganisationLocation, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationLocationNotFound>();
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
