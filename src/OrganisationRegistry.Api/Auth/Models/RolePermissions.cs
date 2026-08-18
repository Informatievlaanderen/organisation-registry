namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;
using Resource = Resource<GlobalResources>;

public static class RolePermissions
{
    private static readonly Dictionary<Role, Resource[]> Map = new()
    {
        [Role.AlgemeenBeheerder] =
        [
            Resource.Create(GlobalResources.OrgOrganisations, CrudOperation.Create),
            Resource.Create(GlobalResources.BodyInfo, CrudOperation.Create),
            Resource.Create(GlobalResources.Reports, CrudOperation.Read),
            Resource.Create(GlobalResources.RefParameters, CrudOperation.Read | CrudOperation.Write),
            Resource.Create(GlobalResources.Imports, CrudOperation.Write),
            Resource.Create(GlobalResources.Delegations, CrudOperation.Read | CrudOperation.Write | CrudOperation.Delete),
        ],
        [Role.DecentraalBeheerder] =
        [
            Resource.Create(GlobalResources.Reports, CrudOperation.Read),
        ],

        [Role.VlimpersBeheerder] =
        [
            Resource.Create(GlobalResources.Reports, CrudOperation.Read),
            Resource.Create(GlobalResources.Imports, CrudOperation.Write),
        ],

        [Role.OrgaanBeheerder] =
        [
            Resource.Create(GlobalResources.Reports, CrudOperation.Read),
        ],

        [Role.RegelgevingBeheerder] =
        [
            Resource.Create(GlobalResources.Reports, CrudOperation.Read),
        ],
    };

    /// <summary>
    /// Returns every permission a role holds, flattened into "<c>resource:operation</c>"
    /// strings (e.g. <c>"ref.parameters:read"</c>). Used by <c>MeController</c> to tell
    /// the frontend what the current user is allowed to do. Returns an empty sequence
    /// for a role with no configured permissions.
    /// </summary>
    public static IEnumerable<string> Resolve(Role role)
        => Map.TryGetValue(role, out var permissions)
            ? permissions.SelectMany(p => p.ToPermissionStrings()).Distinct()
            : [];

    /// <summary>
    /// Checks whether <paramref name="role"/> is allowed to perform all of
    /// <paramref name="requiredOperations"/> on <paramref name="resource"/>.
    /// Fails if the role has no configured permissions, has no entry for that
    /// resource, or has an entry that's missing one or more of the required
    /// operation flags. Extra operations beyond what's required are fine -
    /// this checks "at least", not "exactly".
    /// </summary>
    public static bool Validate(
        Role role,
        GlobalResources resource,
        CrudOperation requiredOperations,
        ILogger? logger = null)
    {
        if (!Map.TryGetValue(role, out var permissions))
            return false;

        var ret = permissions.Any(p =>
            p.Name == resource &&
            p.Operations.HasFlag(requiredOperations));

        logger?.LogInformation($"Role: {role} | Resource: {resource} | OPS: {requiredOperations} | permissions: {permissions}");

        return ret;
    }

    /// <summary>
    /// Whether <paramref name="role"/> has any permissions configured at all in
    /// <see cref="Map"/>, regardless of resource.
    /// </summary>
    public static bool IsConfigured(Role role)
        => Map.ContainsKey(role);
}
