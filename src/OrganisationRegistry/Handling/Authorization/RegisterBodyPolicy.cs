namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Organisation;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for registering a new body (orgaan). Access is
/// gated by the <see cref="Permission.CanManageBodies"/> permission: a holder of
/// an unrestricted grant (e.g. AlgemeenBeheerder or OrgaanBeheerder) may register
/// any body; a holder of a restricted grant (DecentraalBeheerder) may only
/// register a body for their own organisation (or a child organisation in scope).
///
/// Register targets an organisation by id (the body does not yet exist), so the
/// own-organisation check is expressed against the organisation id rather than a
/// <see cref="Infrastructure.Authorization.Restrictions.BodyContext"/>.
/// </summary>
public class RegisterBodyPolicy : ISecurityPolicy
{
    private readonly OrganisationId? _organisationId;

    public RegisterBodyPolicy(OrganisationId? organisationId)
    {
        _organisationId = organisationId;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsSatisfiedFor(Permission.CanManageBodies))
            return AuthorizationResult.Success();

        if (_organisationId is { } organisationId &&
            user.HasPermission(Permission.CanManageBodies) &&
            user.IsDecentraalBeheerderForOrganisation((Guid)organisationId))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    public override string ToString()
        => "Geen machtiging op organisatie";
}
