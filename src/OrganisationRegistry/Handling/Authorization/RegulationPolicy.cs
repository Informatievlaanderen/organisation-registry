namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation regulations. Access
/// is driven entirely by the <see cref="Permission.CanManageRegulations"/>
/// permission (unrestricted for the roles that hold it).
/// </summary>
public class RegulationPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(Permission.CanManageRegulations)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op regelgeving";
}
