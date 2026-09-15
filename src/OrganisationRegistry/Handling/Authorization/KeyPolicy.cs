namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation keys. Access is
/// driven entirely by the <see cref="Permission.CanManageKeys"/> permission and
/// its (optional) restriction, evaluated against a two-axis <see cref="KeyContext"/>:
/// the organisation's Vlimpers-management status and the keytype ids involved.
///
/// A holder of an unrestricted <c>CanManageKeys</c> grant (e.g. AlgemeenBeheerder)
/// always passes; a restricted holder (e.g. VlimpersBeheerder) only passes when the
/// organisation is under Vlimpers management AND every keytype is on the allow-list.
/// </summary>
public class KeyPolicy : ISecurityPolicy
{
    private readonly bool _isUnderVlimpersManagement;
    private readonly Guid[] _keyTypeIds;

    public KeyPolicy(bool isUnderVlimpersManagement, params Guid[] keyTypeIds)
    {
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
        _keyTypeIds = keyTypeIds;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageKeys,
            new UserContext(user),
            new KeyContext(_isUnderVlimpersManagement, _keyTypeIds))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op sleutel";
}
