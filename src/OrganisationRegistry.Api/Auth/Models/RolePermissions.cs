namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Source of truth for role -> permission mapping. Each <see cref="Role"/> maps to a
/// fixed set of <see cref="GlobalPermission"/> entries, each pairing a
/// <see cref="ResourceDefinition"/> with the <see cref="CrudOperation"/> flags that
/// role is allowed to perform on it. Consumed by <see cref="OrganisationRegistry.Api.Infrastructure.Configuration.OrAuthMiddleware"/>
/// (via <see cref="Validate"/>) to authorize requests, and by <c>MeController</c>
/// (via <see cref="Resolve"/>) to expose a user's permissions as strings.
/// </summary>
/// <remarks>
/// Convention: within a role's array, each <see cref="ResourceDefinition"/> appears at
/// most once, with all its granted operations OR'd together into a single
/// <see cref="GlobalPermission"/>. <see cref="Validate"/> relies on this - it does not
/// aggregate multiple entries for the same resource.
/// </remarks>
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
        ResourceDefinition resource,
        CrudOperation requiredOperations,
        ILogger? logger = null)
    {
        if (!Map.TryGetValue(role, out var permissions))
            return false;

        var ret = permissions.Any(p =>
            p.Resource == resource &&
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
