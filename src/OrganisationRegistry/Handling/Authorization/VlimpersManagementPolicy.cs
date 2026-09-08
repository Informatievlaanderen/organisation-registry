namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for placing an organisation under (or releasing it
/// from) Vlimpers management. Access is driven entirely by the
/// <see cref="Permission.CanManageVlimpers"/> permission, which is held only by
/// AlgemeenBeheerder (and the equivalent admin scope). Toggling Vlimpers management is
/// never delegated to a VlimpersBeheerder, DecentraalBeheerder or CjmBeheerder.
/// </summary>
public class VlimpersManagementPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(Permission.CanManageVlimpers)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op Vlimpersbeheer";
}
