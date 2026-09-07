namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Infrastructure.Authorization.Restrictions;
using Organisation.Exceptions;

/// <summary>
/// Role-independent authorization for managing organisation labels. Access is
/// driven entirely by the <see cref="Permission.CanManageLabels"/>
/// permission and its (optional) restrictions, evaluated against a
/// <see cref="LabelContext"/> (the organisation's Vlimpers-management status and
/// the labeltype ids involved) and an <see cref="OrganisationContext"/>.
///
/// A holder of an unrestricted grant (e.g. AlgemeenBeheerder or CjmBeheerder)
/// always passes; a VlimpersBeheerder only passes when the organisation is under
/// Vlimpers management AND every labeltype is Vlimpers-allowed; a
/// DecentraalBeheerder passes for their own organisation unless it is under
/// Vlimpers management with a Vlimpers-typed label.
/// </summary>
public class LabelPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly bool _isUnderVlimpersManagement;
    private readonly Guid[] _labelTypeIds;

    private LabelPolicy(string ovoNumber, bool isUnderVlimpersManagement, params Guid[] labelTypeIds)
    {
        _ovoNumber = ovoNumber;
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
        _labelTypeIds = labelTypeIds;
    }

    public static LabelPolicy ForCreate(string ovoNumber, bool isUnderVlimpersManagement, params Guid[] labelTypeIds)
        => new(ovoNumber, isUnderVlimpersManagement, labelTypeIds);

    public static LabelPolicy ForUpdate(string ovoNumber, bool isUnderVlimpersManagement, Guid oldLabelTypeId, Guid newLabelTypeId)
        => new(ovoNumber, isUnderVlimpersManagement, oldLabelTypeId, newLabelTypeId);

    public AuthorizationResult Check(IUser user)
        => user.IsSatisfiedFor(
            Permission.CanManageLabels,
            new UserContext(user),
            new OrganisationContext(_ovoNumber),
            new LabelContext(_isUnderVlimpersManagement, _labelTypeIds))
            ? AuthorizationResult.Success()
            : AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

    public override string ToString()
        => "Geen machtiging op labeltype.";
}
