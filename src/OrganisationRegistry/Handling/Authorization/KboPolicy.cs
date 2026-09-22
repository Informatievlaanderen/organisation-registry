namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for coupling an organisation to (or decoupling it
/// from) the KBO. Access is driven entirely by the <see cref="Permission.CanManageKbo"/>
/// permission, which is held only by AlgemeenBeheerder (and the equivalent admin scope).
/// The KBO coupling is never delegated to a DecentraalBeheerder or CjmBeheerder.
/// </summary>
public class KboPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(Permission.CanManageKbo)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op KBO-koppeling";
}
