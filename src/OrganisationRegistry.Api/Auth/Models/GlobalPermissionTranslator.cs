namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Translates the fine-grained internal <see cref="Permission"/> grants for a role
/// (as defined in <see cref="RolePermissionMap"/>) into the "&lt;resource&gt;:&lt;operation&gt;"
/// strings the front-end uses to decide which nav tabs and screens to show
/// (e.g. "parameters:read", "parameters.locations:write", "bodies.mandates:read").
///
/// This is driven directly by <see cref="RolePermissionMap"/> — the actual source of
/// truth for what a role may do — instead of a second, hand-maintained list, so new
/// <c>Parameters*</c> / <c>Bodies*</c> permissions granted there automatically show up
/// on <c>/v1/me</c> without needing a matching manual edit here.
///
/// Only the <c>Parameters*Read/Write/Delete</c> and <c>CanManageBodies</c> /
/// <c>BodiesCanManage*</c> families are translated: those are the permissions that
/// gate top-level admin screens. Per-organisation permissions (e.g. <c>CanManageKeys</c>,
/// <c>CanManageCapacities</c>) are scoped to an individual organisation's detail page
/// and are not relevant to global nav visibility, so they are intentionally not
/// translated here.
/// </summary>
public static class GlobalPermissionTranslator
{
    public static IEnumerable<string> Translate(PermissionSet permissions)
    {
        var result = new HashSet<string>();
        var hasBodies = false;
        var hasParametersRead = false;
        var hasParametersWrite = false;

        foreach (var entry in permissions)
        {
            var name = entry.Permission.ToString();

            if (name == nameof(Permission.CanManageBodies))
            {
                hasBodies = true;
                continue;
            }

            if (name.StartsWith("BodiesCanManage"))
            {
                var sub = name["BodiesCanManage".Length..].ToLowerInvariant();
                result.Add($"bodies.{sub}:read");
                result.Add($"bodies.{sub}:write");
                hasBodies = true;
                continue;
            }

            if (!name.StartsWith("Parameters"))
                continue;

            var (resource, operation) = SplitParametersPermission(name);
            if (operation is null)
                continue;

            result.Add($"parameters.{resource}:{operation}");
            hasParametersRead = hasParametersRead || operation == "read";
            hasParametersWrite = hasParametersWrite || operation is "write" or "delete";
        }

        if (hasBodies)
        {
            result.Add("bodies:read");
            result.Add("bodies:write");
        }

        if (hasParametersRead)
            result.Add("parameters:read");

        if (hasParametersWrite)
            result.Add("parameters:write");

        return result;
    }

    /// <summary>
    /// Splits a <c>Parameters{Resource}{Read|Write|Delete}</c> permission name into
    /// its lowercase resource segment and operation. Returns a <c>null</c> operation
    /// when the name doesn't match the expected suffix (defensive; every current
    /// <c>Parameters*</c> member does).
    /// </summary>
    private static (string Resource, string? Operation) SplitParametersPermission(string name)
    {
        const string prefix = "Parameters";

        foreach (var suffix in new[] { "Read", "Write", "Delete" })
        {
            if (!name.EndsWith(suffix) || name.Length <= prefix.Length + suffix.Length)
                continue;

            var resource = name[prefix.Length..^suffix.Length].ToLowerInvariant();
            return (resource, suffix.ToLowerInvariant());
        }

        return (string.Empty, null);
    }
}
