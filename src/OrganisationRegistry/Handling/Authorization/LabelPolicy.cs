namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class LabelPolicy : ISecurityPolicy
{
    private readonly string _ovoNumber;
    private readonly bool _isUnderVlimpersManagement;
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly Guid[] _labelTypeIds;

    private LabelPolicy(
        string ovoNumber,
        bool isUnderVlimpersManagement,
        IOrganisationRegistryConfiguration configuration,
        params Guid[] labelTypeIds
    )
    {
        _ovoNumber = ovoNumber;
        _isUnderVlimpersManagement = isUnderVlimpersManagement;
        _configuration = configuration;
        _labelTypeIds = labelTypeIds;
    }

    public static LabelPolicy ForCreate(string ovoNumber, bool isUnderVlimpersManagement, IOrganisationRegistryConfiguration configuration, params Guid[] labelTypeIds)
        => new(ovoNumber, isUnderVlimpersManagement, configuration, labelTypeIds);

    public static LabelPolicy ForUpdate(string ovoNumber, bool isUnderVlimpersManagement, IOrganisationRegistryConfiguration configuration, Guid oldLabelTypeId, Guid newLabelTypeId)
        => new(ovoNumber, isUnderVlimpersManagement, configuration, oldLabelTypeId, newLabelTypeId);

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (_isUnderVlimpersManagement &&
            user.IsInAnyOf(Role.VlimpersBeheerder) && AreAllLabelsofTypeVlimpers(_labelTypeIds))
            return AuthorizationResult.Success();

        if (!user.IsDecentraalBeheerderForOrganisation(_ovoNumber))
            return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

        if (_isUnderVlimpersManagement && AreAnyLabelsofTypeVlimpers(_labelTypeIds))
            return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));

        return AuthorizationResult.Success();
    }

    private bool AreAllLabelsofTypeVlimpers(IEnumerable<Guid> labelTypeIds)
        => labelTypeIds.All(
            labelTypeId => _configuration.Authorization.LabelIdsAllowedForVlimpers.Contains(labelTypeId)
        );

    private bool AreAnyLabelsofTypeVlimpers(IEnumerable<Guid> labelTypeIds)
        => labelTypeIds.Any(
            labelTypeId => _configuration.Authorization.LabelIdsAllowedForVlimpers.Contains(labelTypeId)
        );

    public override string ToString()
        => "Geen machtiging op labeltype.";
}
