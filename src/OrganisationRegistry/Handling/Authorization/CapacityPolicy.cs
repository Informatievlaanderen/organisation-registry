namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation capacities. Access
/// is driven entirely by the <see cref="Permission.CanManageCapacities"/>
/// permission and its (optional) restrictions, evaluated against a
/// <see cref="CapacityContext"/> and an <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted <c>CanManageCapacities</c> grant (e.g.
/// AlgemeenBeheerder) always passes; a restricted holder only passes when the
/// organisation and capacity id match the configured rules for that role.
/// </summary>
public class CapacityPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly Guid _organisationCapacityId;

    public CapacityPolicy(string ovoNumber, Guid organisationCapacityId)
    {
        _ovoNumber = ovoNumber;
        _organisationCapacityId = organisationCapacityId;
    }

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageCapacities,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new CapacityContext(_organisationCapacityId))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op hoedanigheid";
}
