namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class OrganisationClassificationTypePolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationClassificationTypeId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public OrganisationClassificationTypePolicy(
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration,
        Guid organisationClassificationTypeId)
    {
        _ovoNumber = ovoNumber;
        _configuration = configuration;
        _organisationClassificationTypeId = organisationClassificationTypeId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        var organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder;

        if (user.IsInRole(Role.RegelgevingBeheerder) &&
            organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationClassificationTypeId))
            return AuthorizationResult.Success();

        if (user.IsOrganisatieBeheerderFor(_ovoNumber) &&
            !organisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.Contains(_organisationClassificationTypeId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
