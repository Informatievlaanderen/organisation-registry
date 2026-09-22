namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for creating/updating a <c>Persoon</c>. A person is
/// not organisation-scoped (unlike contacts/functions/locations, ...), so access is
/// driven entirely by the <see cref="Permission.PeopleWrite"/> permission, which is
/// held unrestricted only by AlgemeenBeheerder (and Developer). There is no delete
/// variant: people can never be deleted via the API.
/// </summary>
public class PeoplePolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(Permission.PeopleWrite)
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op persoon";
}
