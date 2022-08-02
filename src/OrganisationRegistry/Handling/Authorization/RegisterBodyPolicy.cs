namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Organisation;
using Organisation.Exceptions;

public class RegisterBodyPolicy : ISecurityPolicy
{
    private readonly OrganisationId? _organisationId;

    public RegisterBodyPolicy(OrganisationId? organisationId)
    {
        _organisationId = organisationId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.OrgaanBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (_organisationId is { } organisationId && user.IsDecentraalBeheerderForOrganisation((Guid)organisationId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie";
}
