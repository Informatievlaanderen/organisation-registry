namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Configuration;
using Infrastructure.Authorization;
using Organisation.Exceptions;

public class OrganisationCapacityTypePolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationCapacityTypeId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public OrganisationCapacityTypePolicy(
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration,
        Guid organisationCapacityTypeId)
    {
        _ovoNumber = ovoNumber;
        _configuration = configuration;
        _organisationCapacityTypeId = organisationCapacityTypeId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        var organisationCapacityTypeIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.OrganisationCapacityTypeIdsOwnedByRegelgevingDbBeheerder;

        if (user.IsInRole(Role.RegelgevingBeheerder) &&
            organisationCapacityTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityTypeId))
            return AuthorizationResult.Success();

        if (user.IsOrganisatieBeheerderFor(_ovoNumber) &&
            !organisationCapacityTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationCapacityTypeId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
