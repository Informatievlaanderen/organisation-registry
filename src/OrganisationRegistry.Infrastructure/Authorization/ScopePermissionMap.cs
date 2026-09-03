namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

/// <summary>
/// Static translation from ACM/IDM Client-Credentials <c>scope</c> claim values
/// to the internal <see cref="PermissionSet"/> language.
///
/// Unknown / unmapped scopes fail closed (return <see cref="PermissionSet.Empty"/>)
/// and emit a Serilog warning throttled to once per scope per process. Only
/// scopes registered here participate in permission translation; other tokens
/// carrying unrelated scope values are ignored silently.
/// </summary>
public static class ScopePermissionMap
{
    private static readonly IReadOnlyDictionary<string, PermissionSet> Map =
        new Dictionary<string, PermissionSet>(StringComparer.Ordinal)
        {
            [AcmIdmConstants.Scopes.CjmBeheerder] = PermissionSet.Of(
                Permission.CanAddBodies,
                Permission.CanEditBodies),

            [AcmIdmConstants.Scopes.OrafinBeheerder] = PermissionSet.Of(
                Permission.CanReadOrafin),

            [AcmIdmConstants.Scopes.Info] = PermissionSet.Of(
                Permission.CanReadInfoEndpoints),

            // TestClient is used by integration tests and dev harnesses to
            // impersonate a full admin. Kept aligned with the historical
            // WellknownUsers.TestClient → Role.AlgemeenBeheerder mapping.
            [AcmIdmConstants.Scopes.TestClient] = PermissionSet.Of(
                Permission.CanEditChildren,
                Permission.CanAddBodies,
                Permission.CanEditBodies,
                Permission.CanRegisterBodies,
                Permission.CanAddLocations,
                Permission.CanAddContacts,
                Permission.CanManageKeys,
                Permission.CanManageLabels,
                Permission.CanManageCapacities,
                Permission.CanManageFormalFrameworks,
                Permission.CanManageOrganisationClassifications,
                Permission.CanManageRegulations,
                Permission.CanImport,
                Permission.CanEditVlimpers,
                Permission.CanEditDelegations,
                Permission.CanReadConfiguration,
                Permission.CanEditOrganisationLabels),
        };

    private static readonly ConcurrentDictionary<string, byte> LoggedUnknownScopes =
        new(StringComparer.Ordinal);

    /// <summary>
    /// Returns the <see cref="PermissionSet"/> for a single scope string.
    /// Scopes not present in the map return <see cref="PermissionSet.Empty"/>
    /// without logging — only registered-but-unmapped scopes would log, and
    /// currently every registered scope is mapped, so this path is silent.
    /// </summary>
    public static PermissionSet For(string? scope, ILogger? logger = null)
    {
        if (string.IsNullOrEmpty(scope))
            return PermissionSet.Empty;

        return Map.TryGetValue(scope, out var permissions)
            ? permissions
            : PermissionSet.Empty;
    }

    /// <summary>
    /// Returns the union of permissions across all supplied scopes. Only
    /// scopes recognised by <see cref="AcmIdmConstants.Scopes"/> contribute;
    /// unrecognised scopes are ignored silently (they belong to unrelated
    /// resource servers). A throttled warning is emitted once per process
    /// per scope that starts with the org-registry prefix but is unmapped.
    /// </summary>
    public static PermissionSet For(IEnumerable<string>? scopes, ILogger? logger = null)
    {
        if (scopes is null)
            return PermissionSet.Empty;

        var union = PermissionSet.Empty;
        foreach (var scope in scopes)
        {
            if (string.IsNullOrEmpty(scope))
                continue;

            if (Map.TryGetValue(scope, out var permissions))
            {
                union = union.Union(permissions);
                continue;
            }

            if (scope.StartsWith("dv_organisatieregister_", StringComparison.Ordinal)
                && LoggedUnknownScopes.TryAdd(scope, 0))
            {
                logger?.LogWarning(
                    "Unknown organisation-registry scope {Scope} encountered during permission translation; ignoring (fail-closed).",
                    scope);
            }
        }

        return union;
    }

    /// <summary>Test-only: clears the unknown-scope throttle memory.</summary>
    internal static void ResetThrottleState() => LoggedUnknownScopes.Clear();
}
