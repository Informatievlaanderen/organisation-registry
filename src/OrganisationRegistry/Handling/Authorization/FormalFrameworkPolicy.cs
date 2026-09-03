namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation formal frameworks.
/// Access is driven entirely by the <see cref="Permission.CanManageFormalFrameworks"/>
/// permission and its (optional) restrictions, evaluated against a
/// <see cref="FormalFrameworkContext"/> and an <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted <c>CanManageFormalFrameworks</c> grant (e.g.
/// AlgemeenBeheerder) always passes; a restricted holder only passes when the
/// organisation and formal framework id match the configured rules for that role.
/// </summary>
public class FormalFrameworkPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _formalFrameworkId;

    public FormalFrameworkPolicy(string ovoNumber, Guid formalFrameworkId)
    {
        _ovoNumber = ovoNumber;
        _formalFrameworkId = formalFrameworkId;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageFormalFrameworks,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new FormalFrameworkContext(_formalFrameworkId))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op toepassingsgebied";
}
