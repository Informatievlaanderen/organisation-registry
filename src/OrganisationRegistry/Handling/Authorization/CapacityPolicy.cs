namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class CapacityPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationCapacityId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public CapacityPolicy(
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration,
        Guid organisationCapacityId)
    {
        _ovoNumber = ovoNumber;
        _configuration = configuration;
        _organisationCapacityId = organisationCapacityId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        var organisationCapacityIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder;

        if (user.IsInAnyOf(Role.RegelgevingBeheerder) &&
            organisationCapacityIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityId))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderForOrganisation(_ovoNumber) &&
            !organisationCapacityIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op hoedanigheid";
}
