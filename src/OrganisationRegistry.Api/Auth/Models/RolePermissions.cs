namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization;

public static class RolePermissions
{
    // Permissions here have no dedicated entry in RolePermissionMap (they gate
    // command controllers directly, e.g. via [OrganisationRegistryAuthorize] with
    // no RequiredPermissions, or are entirely UI-facing conventions), so they stay
    // hand-maintained. Parameters/Bodies admin-screen permissions, organisations:create,
    // bodies:create, imports, delegations:read/write/delete and system(.*) are NOT
    // listed here — they are derived from RolePermissionMap by
    // GlobalPermissionTranslator so they can't drift out of sync with the real
    // grants. "reports" has no backing Permission at all (no reports endpoint is
    // gated yet), so it remains a hand-maintained, per-role UI convention.
    private static readonly Dictionary<Role, GlobalPermission[]> Map = new()
    {
        [Role.AlgemeenBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],

        [Role.Developer] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
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
            ? globalPermissions.SelectMany(p => p.ToPermissionStrings()).ToList()
            : new List<string>();

        var derived = GlobalPermissionTranslator.Translate(permissions);

        var result = manual.Concat(derived).Distinct().ToList();

        // "reports" has no backing Permission (see Map above), so its bare
        // aggregate flag can't be derived by GlobalPermissionTranslator; add it
        // here whenever at least one reports:<operation> string is present.
        if (result.Any(p => p.StartsWith("reports:")))
            result.Add("reports");

        return result.Distinct().OrderBy(p => p, System.StringComparer.Ordinal);
    }

    public static bool IsConfigured(Role role) =>
        Map.ContainsKey(role) || RolePermissionMap.For(role).Any();
}

