namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Organisation;
using Organisation.Exceptions;

public class EditDelegationPolicy : ISecurityPolicy
{
    private readonly OrganisationId _organisationId;
    private readonly Guid _bodyId;

    public EditDelegationPolicy(OrganisationId organisationId, Guid bodyId)
    {
        _organisationId = organisationId;
        _bodyId = bodyId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.OrgaanBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderForOrganisation((Guid)_organisationId))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderForBody(_bodyId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie of orgaan";
}
