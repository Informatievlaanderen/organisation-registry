namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization;

public static class RolePermissions
{
    // Permissions here have no dedicated entry in RolePermissionMap (they gate
    // command controllers directly, e.g. via [OrganisationRegistryAuthorize] with
    // no RequiredPermissions, or are entirely UI-facing conventions), so they stay
    // hand-maintained. Parameters/Bodies admin-screen permissions, org.organisations:create,
    // body.info:create and imports are NOT listed here — they are derived from
    // RolePermissionMap by GlobalPermissionTranslator so they can't drift out of
    // sync with the real grants.
    private static readonly Dictionary<Role, GlobalPermission[]> Map = new()
    {
        [Role.AlgemeenBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
            new GlobalPermission("delegations", CrudOperation.Read | CrudOperation.Write | CrudOperation.Delete),
            new GlobalPermission("system.statistics", CrudOperation.Read),
            new GlobalPermission("system.events", CrudOperation.Read),
            new GlobalPermission("system.kbo-terminated", CrudOperation.Read),
        ],

        [Role.DecentraalBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],

        [Role.VlimpersBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],

        [Role.OrgaanBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],

        [Role.RegelgevingBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],
    };

    /// <summary>
    /// Resolves the full "&lt;resource&gt;:&lt;operation&gt;" permission-string list for
    /// <c>/v1/me</c>. <paramref name="permissions"/> must be resolved via the
    /// config-aware <see cref="RolePermissionMap.For(System.Collections.Generic.IEnumerable{Role},OrganisationRegistry.Infrastructure.Configuration.IOrganisationRegistryConfiguration,Microsoft.Extensions.Logging.ILogger)"/>
    /// overload (i.e. the same <see cref="OrganisationRegistry.Infrastructure.Authorization.IUser.Permissions"/>
    /// the caller is already authenticated with) so restricted grants (Vlimpers/Decentraal)
    /// are taken into account — the config-less overload would silently hide them.
    /// </summary>
    public static IEnumerable<string> Resolve(Role role, PermissionSet permissions)
    {
        var manual = Map.TryGetValue(role, out var globalPermissions)
            ? globalPermissions.SelectMany(p => p.ToPermissionStrings())
            : Enumerable.Empty<string>();

        var derived = GlobalPermissionTranslator.Translate(permissions);

        return manual.Concat(derived).Distinct();
    }

    public static bool IsConfigured(Role role) =>
        Map.ContainsKey(role) || RolePermissionMap.For(role).Any();
}

