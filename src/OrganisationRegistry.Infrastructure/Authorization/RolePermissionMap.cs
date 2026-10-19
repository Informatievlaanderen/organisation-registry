namespace OrganisationRegistry.Infrastructure.Authorization;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Configuration;
using Microsoft.Extensions.Logging;
using RolePermissions;

/// <summary>
/// Static translation from <see cref="Role"/> claims (edit-api and token-exchange
/// entry points) to the internal <see cref="PermissionSet"/> language.
///
/// Unknown / unmapped roles fail closed (return <see cref="PermissionSet.Empty"/>)
/// and emit a Serilog warning throttled to once per role per process.
/// </summary>
public static class RolePermissionMap
{
    private static readonly IReadOnlyDictionary<Role, IRolePermissionProvider> Providers =
        new Dictionary<Role, IRolePermissionProvider>
        {
            [Role.AlgemeenBeheerder] = new AlgemeenBeheerderPermissions(),
            [Role.OrgaanBeheerder] = new OrgaanBeheerderPermissions(),
            [Role.VlimpersBeheerder] = new VlimpersBeheerderPermissions(),
            [Role.DecentraalBeheerder] = new DecentraalBeheerderPermissions(),
            [Role.RegelgevingBeheerder] = new RegelgevingBeheerderPermissions(),
            [Role.CjmBeheerder] = new CjmBeheerderPermissions(),
            [Role.Orafin] = new OrafinPermissions(),
            [Role.Developer] = new DeveloperPermissions(),
            [Role.AutomatedTask] = new AutomatedTaskPermissions(),
            [Role.VoMedewerker] = new VoMedewerkerPermissions(),
            [Role.VoDecentraalBeheerder] = new VoDecentraalBeheerderPermissions(),
        };

    private static readonly ConcurrentDictionary<Role, byte> LoggedUnknownRoles = new();

    private static readonly ConcurrentDictionary<string, byte> LoggedUnknownRoleNames = new();

    public static PermissionSet For(Role role, ILogger? logger = null)
    {
        if (Providers.TryGetValue(role, out var provider))
            return provider.Static;

        if (LoggedUnknownRoles.TryAdd(role, 0))
            logger?.LogWarning(
                "Unknown Role {Role} encountered during permission translation; returning empty permission set (fail-closed).",
                role);

        return PermissionSet.Empty;
    }

    public static PermissionSet For(string roleName, ILogger? logger = null)
    {
        if (Enum.TryParse<Role>(roleName, out var role))
            return For(role, logger);

        if (LoggedUnknownRoleNames.TryAdd(roleName, 0))
            logger?.LogWarning(
                "Unknown Role name {RoleName} encountered during permission translation; returning empty permission set (fail-closed).",
                roleName);

        return PermissionSet.Empty;
    }

    public static PermissionSet For(IEnumerable<string>? roleNames, ILogger? logger = null)
    {
        if (roleNames is null)
            return PermissionSet.Empty;

        var union = PermissionSet.Empty;
        foreach (var roleName in roleNames)
            union = union.Union(For(roleName, logger));

        return union;
    }

    public static PermissionSet For(
        IEnumerable<string>? roleNames,
        IOrganisationRegistryConfiguration configuration,
        ILogger? logger = null)
    {
        if (roleNames is null)
            return PermissionSet.Empty;

        var union = For(roleNames, logger);

        foreach (var roleName in roleNames)
            if (Enum.TryParse<Role>(roleName, out var role))
                union = union.Union(RestrictedGrantsFor(role, configuration));

        return union;
    }

    public static PermissionSet For(IEnumerable<Role>? roles, ILogger? logger = null)
    {
        if (roles is null)
            return PermissionSet.Empty;

        var union = PermissionSet.Empty;
        foreach (var role in roles)
            union = union.Union(For(role, logger));

        return union;
    }

    /// <summary>
    /// Config-aware translation. Layers data-driven restricted grants (whose
    /// restrictions depend on runtime configuration, e.g. the Vlimpers-allowed
    /// keytype ids) on top of the static role → permission mapping. Use this
    /// overload wherever a caller needs restriction enforcement (SecurityService);
    /// the config-less overload is fine where only permission presence matters.
    /// </summary>
    public static PermissionSet For(
        IEnumerable<Role>? roles,
        IOrganisationRegistryConfiguration configuration,
        ILogger? logger = null)
    {
        if (roles is null)
            return PermissionSet.Empty;

        var roleList = roles as IReadOnlyCollection<Role> ?? roles.ToList();
        var union = For(roleList, logger);

        foreach (var role in roleList)
            union = union.Union(RestrictedGrantsFor(role, configuration));

        return union;
    }

    private static PermissionSet RestrictedGrantsFor(
        Role role,
        IOrganisationRegistryConfiguration configuration)
        => Providers.TryGetValue(role, out var provider)
            ? provider.Restricted(configuration)
            : PermissionSet.Empty;

    /// <summary>Test-only: clears the unknown-role throttle memory.</summary>
    internal static void ResetThrottleState() => LoggedUnknownRoles.Clear();
}
