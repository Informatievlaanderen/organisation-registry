namespace OrganisationRegistry.Handling.Authorization;

using System;
using System.Linq;
using Infrastructure.Authorization;
using Infrastructure.Configuration;
using Organisation.Exceptions;

public class KeyPolicy : ISecurityPolicy
{
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly Guid[] _keyTypeIds;
    private readonly string _ovoNumber;

    public KeyPolicy(
        string ovoNumber,
        IOrganisationRegistryConfiguration configuration,
        params Guid[] keyTypeIds)
    {
        _ovoNumber = ovoNumber;
        _keyTypeIds = keyTypeIds;
        _configuration = configuration;
    }

    public AuthorizationResult Check(IUser user)
    {
        if (user.IsInAnyOf(Role.AlgemeenBeheerder, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        var containsVlimpersKey = ContainsVlimpersKey(_keyTypeIds);
        var containsOrafinKey = ContainsOrafinKey(_keyTypeIds);

        if (containsOrafinKey && user.IsInAnyOf(Role.Orafin, Role.CjmBeheerder))
            return AuthorizationResult.Success();

        if (containsVlimpersKey && user.IsInAnyOf(Role.VlimpersBeheerder))
            return AuthorizationResult.Success();

        if (!containsOrafinKey && !containsVlimpersKey &&
            user.IsDecentraalBeheerderForOrganisation(_ovoNumber))
            return AuthorizationResult.Success();

        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));
    }

    private bool ContainsOrafinKey(Guid[] keyTypeIds)
    {
        var keyTypeIdsAllowedByVlimpers = _configuration.Authorization.KeyIdsAllowedOnlyForOrafin;
        return keyTypeIds.Any(
            labelTypeId =>
                keyTypeIdsAllowedByVlimpers.Contains(labelTypeId));
    }

    private bool ContainsVlimpersKey(Guid[] keyTypeIds)
    {
        var keyTypeIdsAllowedByVlimpers = _configuration.Authorization.KeyIdsAllowedForVlimpers;
        return keyTypeIds.Any(
            labelTypeId =>
                keyTypeIdsAllowedByVlimpers.Contains(labelTypeId));
    }

    public override string ToString()
        => "Geen machtiging op sleutel";
}
