namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class FormalFrameworkPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _formalFrameworkId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public FormalFrameworkPolicy(
        string ovoNumber,
        Guid formalFrameworkId,
        IOrganisationRegistryConfiguration configuration)
    {
        _ovoNumber = ovoNumber;
        _formalFrameworkId = formalFrameworkId;
        _configuration = configuration;
    }

    public AuthorizationResult Check(IUser user)
    {
        if(user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        var formalFrameworkIdsOwnedByVlimpers = _configuration.Authorization.FormalFrameworkIdsOwnedByVlimpers;
        var formalFrameworkIdsOwnedByAuditVlaanderen = _configuration.Authorization.FormalFrameworkIdsOwnedByAuditVlaanderen;
        var formalFrameworkIdsOwnedByRegelgevingDbBeheerder = _configuration.Authorization.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder;

        var formalFrameworkIdsExcludedForOrganisatieBeheerder = formalFrameworkIdsOwnedByVlimpers
            .Union(formalFrameworkIdsOwnedByAuditVlaanderen)
            .Union(formalFrameworkIdsOwnedByRegelgevingDbBeheerder);

        if (user.IsInAnyOf(Role.RegelgevingBeheerder) &&
            formalFrameworkIdsOwnedByRegelgevingDbBeheerder.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        if (user.IsInAnyOf(Role.VlimpersBeheerder) &&
            formalFrameworkIdsOwnedByVlimpers.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        if(user.IsDecentraalBeheerderForOrganisation(_ovoNumber) &&
           !formalFrameworkIdsExcludedForOrganisatieBeheerder.Contains(_formalFrameworkId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op toepassingsgebied";
}
