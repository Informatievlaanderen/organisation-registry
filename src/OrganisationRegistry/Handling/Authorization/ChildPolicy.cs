namespace OrganisationRegistry.Handling.Authorization;

using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing an organisation's parent/child
/// structure (adding a daughter, setting or updating a parent). Access is driven
/// entirely by the <see cref="Permission.CanManageChildren"/> permission and its
/// (optional) restrictions, evaluated against the target organisation's OVO number
/// and Vlimpers-management flag.
///
/// A holder of an unrestricted grant (AlgemeenBeheerder, CjmBeheerder, Developer)
/// always passes; a VlimpersBeheerder only passes when the organisation is under
/// Vlimpers management; a DecentraalBeheerder only passes for their own
/// organisation when it is not under Vlimpers management. This mirrors the
/// behaviour of the historical role-based <c>VlimpersPolicy</c>.
/// </summary>
public class ChildPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly bool _isUnderVlimpersManagement;

    public ChildPolicy(string ovoNumber, bool isUnderVlimpersManagement)
    {
        _ovoNumber = ovoNumber;
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageChildren,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new VlimpersManagementContext(_isUnderVlimpersManagement))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op onderliggende organisatie";
}
