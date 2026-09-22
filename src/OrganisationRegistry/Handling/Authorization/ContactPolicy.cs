namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation contacts. Access is
/// driven entirely by the <see cref="Permission.CanManageContacts"/> permission,
/// which is held unrestricted only by AlgemeenBeheerder (and the equivalent
/// admin scope). Unlike buildings/functions/relations, organisation contacts are
/// <em>not</em> delegated to a DecentraalBeheerder for their own organisation.
/// </summary>
public class ContactPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(Permission.CanManageContacts)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op contact";
}
