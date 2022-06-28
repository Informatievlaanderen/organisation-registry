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
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        var organisationCapacityIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.CapacityIdsOwnedByRegelgevingDbBeheerder;

        if (user.IsInRole(Role.RegelgevingBeheerder) &&
            organisationCapacityIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityId))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderFor(_ovoNumber) &&
            !organisationCapacityIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
