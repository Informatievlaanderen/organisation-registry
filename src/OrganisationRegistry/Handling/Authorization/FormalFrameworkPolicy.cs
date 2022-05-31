namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class FormalFrameworkPolicy : ISecurityPolicy
{
    private readonly Func<string> _ovoNumberFunc;
    private readonly Guid _formalFrameworkId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public FormalFrameworkPolicy(
        Func<string> ovoNumberFunc,
        Guid formalFrameworkId,
        IOrganisationRegistryConfiguration configuration)
    {
        _ovoNumberFunc = ovoNumberFunc;
        _formalFrameworkId = formalFrameworkId;
        _configuration = configuration;
    }

    public AuthorizationResult Check(IUser user)
    {
        if(user.IsInRole(Role.AlgemeenBeheerder))
            return AuthorizationResult.Success();

        var formalFrameworkIdsOwnedByVlimpers = _configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers;
        var formalFrameworkIdsOwnedByAuditVlaanderen = _configuration.Authorization.FormalFrameworkIdsOwnedByAuditVlaanderen;
        var formalFrameworkIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder;

        var formalFrameworkIdsExcludedForOrganisatieBeheerder = formalFrameworkIdsOwnedByVlimpers
            .Union(formalFrameworkIdsOwnedByAuditVlaanderen)
            .Union(formalFrameworkIdsOwnedByRegelgevingDbBeheerder);

        if (user.IsInRole(Role.RegelgevingBeheerder) &&
            formalFrameworkIdsOwnedByRegelgevingDbBeheerder.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        if (user.IsInRole(Role.VlimpersBeheerder) &&
            formalFrameworkIdsOwnedByVlimpers.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        if(user.IsOrganisatieBeheerderFor(_ovoNumberFunc()) &&
           !formalFrameworkIdsExcludedForOrganisatieBeheerder.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(new InsufficientRights());
    }
}
