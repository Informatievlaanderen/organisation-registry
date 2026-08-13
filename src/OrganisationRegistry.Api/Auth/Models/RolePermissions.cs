namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Authorization;

public static class RolePermissions
{
    private static readonly Dictionary<Role, GlobalPermission[]> Map = new()
    {
        [Role.AlgemeenBeheerder] =
        [
            new GlobalPermission("org.organisations", CrudOperation.Create),
            new GlobalPermission("body.info", CrudOperation.Create),
            new GlobalPermission("reports", CrudOperation.Read),
            new GlobalPermission("ref.parameters", CrudOperation.Read | CrudOperation.Write),
            new GlobalPermission("imports", CrudOperation.Write),
            new GlobalPermission("delegations", CrudOperation.Read | CrudOperation.Write | CrudOperation.Delete),
        ],

        [Role.DecentraalBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
        ],

        [Role.VlimpersBeheerder] =
        [
            new GlobalPermission("reports", CrudOperation.Read),
            new GlobalPermission("imports", CrudOperation.Write),
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

    public static IEnumerable<string> Resolve(Role role) =>
        Map.TryGetValue(role, out var permissions)
            ? permissions.SelectMany(p => p.ToPermissionStrings()).Distinct()
            : Enumerable.Empty<string>();

    public static bool IsConfigured(Role role) => Map.ContainsKey(role);
}
