namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Organisation.Exceptions;

public class EditBodyPolicy : ISecurityPolicy
{
    private readonly Guid _bodyId;

    public EditBodyPolicy(Guid bodyId)
    {
        _bodyId = bodyId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.OrgaanBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (user.IsDecentraalBeheerderForBody(_bodyId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op orgaan";
}
