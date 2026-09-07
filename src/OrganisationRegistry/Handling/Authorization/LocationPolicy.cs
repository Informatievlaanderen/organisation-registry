namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation locations. Access is
/// driven entirely by the <see cref="Permission.CanManageLocations"/> permission
/// and its (optional) restrictions, evaluated against a
/// <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted <c>CanManageLocations</c> grant (e.g.
/// AlgemeenBeheerder) always passes; a restricted holder (DecentraalBeheerder)
/// only passes for their own organisation (or a child organisation in scope).
/// </summary>
public class LocationPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;

    public LocationPolicy(string ovoNumber)
    {
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageLocations,
            new UserContext(user),
            new OrganisationContext(_ovoNumber))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op locatie";
}
