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
            new GlobalPermission(ResourceDefinition.OrgOrganisations, CrudOperation.Create),
            new GlobalPermission(ResourceDefinition.BodyInfo, CrudOperation.Create),
            new GlobalPermission(ResourceDefinition.Reports, CrudOperation.Read),
            new GlobalPermission(ResourceDefinition.RefParameters, CrudOperation.Read | CrudOperation.Write),
            new GlobalPermission(ResourceDefinition.Imports, CrudOperation.Write),
            new GlobalPermission(ResourceDefinition.Delegations, CrudOperation.Read | CrudOperation.Write | CrudOperation.Delete),
        ],

        [Role.DecentraalBeheerder] =
        [
            new GlobalPermission(ResourceDefinition.Reports, CrudOperation.Read),
        ],

        [Role.VlimpersBeheerder] =
        [
            new GlobalPermission(ResourceDefinition.Reports, CrudOperation.Read),
            new GlobalPermission(ResourceDefinition.Imports, CrudOperation.Write),
        ],

        [Role.OrgaanBeheerder] =
        [
            new GlobalPermission(ResourceDefinition.Reports, CrudOperation.Read),
        ],

        [Role.RegelgevingBeheerder] =
        [
            new GlobalPermission(ResourceDefinition.Reports, CrudOperation.Read),
        ],
    };

    public static IEnumerable<string> Resolve(Role role) =>
        Map.TryGetValue(role, out var permissions)
            ? permissions.SelectMany(p => p.ToPermissionStrings()).Distinct()
            : [];

    public static bool IsConfigured(Role role) => Map.ContainsKey(role);
}
