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

public class WhenRemovingTheKboOrganistionLocation: Specification<DeleteOrganisationLocationCommandHandler, DeleteOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _kboOrganisationLocationId;

    public WhenRemovingTheKboOrganistionLocation(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _kboOrganisationLocationId = Guid.NewGuid();
    }

    protected override DeleteOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(NullLogger<DeleteOrganisationLocationCommandHandler>.Instance, session);

    private DeleteOrganisationLocation DeleteOrganisationLocation
        => new(new OrganisationId(_organisationId), _kboOrganisationLocationId);

    [Fact]
    public async Task ThrowsAnOrganistionLocationIsKboLocationException()
    {
        await Given(OrganisationCreated, KboRegisteredOfficeOrganisationLocationAdded)
            .When(DeleteOrganisationLocation, TestUser.AlgemeenBeheerder)
            .ThenThrows<LocationIsKboLocation>();
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

    private KboRegisteredOfficeOrganisationLocationAdded KboRegisteredOfficeOrganisationLocationAdded
        => new(
            _organisationId,
            _kboOrganisationLocationId,
            Guid.NewGuid(),
            "",
            isMainLocation: false,
            locationTypeId: null,
            locationTypeName: string.Empty,
            validFrom: null,
            validTo: null);
}
