namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation contacts. Access is
/// driven entirely by the <see cref="Permission.CanManageContacts"/> permission
/// and its (optional) restrictions, evaluated against a
/// <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted <c>CanManageContacts</c> grant (e.g.
/// AlgemeenBeheerder) always passes; a restricted holder (DecentraalBeheerder)
/// only passes for their own organisation (or a child organisation in scope),
/// regardless of Vlimpers-management status. VoDecentraalBeheerder never holds
/// this permission.
/// </summary>
public class ContactPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;

    public ContactPolicy(string ovoNumber)
    {
        _ovoNumber = ovoNumber;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageContacts,
            new UserContext(user),
            new OrganisationContext.ByOvoNumber(_ovoNumber))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op contact";
}
