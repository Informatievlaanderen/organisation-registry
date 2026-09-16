namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing an organisation's own data
/// (e.g. its info/description, or any other organisation-scoped permission
/// that needs both the OVO number and the Vlimpers-management flag). Access
/// is driven entirely by the supplied <see cref="Permission"/> and its
/// (optional) restrictions, evaluated against an <see cref="OrganisationContext"/>
/// and a <see cref="VlimpersManagementContext"/>.
///
/// A holder of an unrestricted grant (e.g. AlgemeenBeheerder, CjmBeheerder)
/// always passes; a restricted holder only passes when the restriction is
/// satisfied (e.g. DecentraalBeheerder for their own organisation as long as
/// it is not under Vlimpers management, or VlimpersBeheerder for an
/// organisation that is under Vlimpers management).
/// </summary>
public class OrganisationPolicy : ISecurityPolicy
{
    private readonly Permission _permission;
    private readonly string _ovoNumber;
    private readonly bool _isUnderVlimpersManagement;

    public OrganisationPolicy(Permission permission, string ovoNumber, bool isUnderVlimpersManagement)
    {
        _permission = permission;
        _ovoNumber = ovoNumber;
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            _permission,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new VlimpersManagementContext(_isUnderVlimpersManagement))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op organisatie";
}
