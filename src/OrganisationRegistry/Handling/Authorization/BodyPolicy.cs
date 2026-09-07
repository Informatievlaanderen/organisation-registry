namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing a body (orgaan) and its
/// sub-features (contacts, seats, mandates, lifecycle phases, organisations,
/// classifications, formal frameworks, MEP-decreet, ...). Access is driven
/// entirely by the supplied <see cref="Permission"/> and its (optional)
/// restrictions, evaluated against a <see cref="BodyContext"/>.
///
/// A holder of an unrestricted grant (e.g. AlgemeenBeheerder or OrgaanBeheerder)
/// always passes; a restricted holder (DecentraalBeheerder) only passes for a
/// body that belongs to their own organisation or a child organisation in scope.
/// </summary>
public class BodyPolicy : ISecurityPolicy
{
    private readonly Permission _permission;
    private readonly Guid _bodyId;

    public BodyPolicy(Permission permission, Guid bodyId)
    {
        _permission = permission;
        _bodyId = bodyId;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            _permission,
            new UserContext(user),
            new BodyContext(_bodyId))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op orgaan";
}
