namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Text;
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
/// The <c>Parameters*Write/Delete</c>, <c>CanManageBodies</c> /
/// <c>BodiesCanManage*</c> and <c>People*</c> families are translated, as well
/// as the top-level "can this role create/manage organisations or bodies at
/// all" flags (<c>org.organisations:create</c>, <c>body.info:create</c>) and
/// <c>imports</c>.
/// Per-organisation permissions (e.g. <c>CanManageKeys</c>, <c>CanManageCapacities</c>)
/// are scoped to an individual organisation's detail page and are not relevant to
/// global nav visibility, so they are intentionally not translated here.
///
/// There is no <c>Parameters*Read</c> permission: reading a master-data list is
/// open to any authenticated backoffice user (see the parameter list
/// controllers), so it carries no per-role signal for nav visibility. Only the
/// write/delete grants (which remain AlgemeenBeheerder/Developer-only today)
/// are translated into <c>parameters.&lt;resource&gt;:write</c>,
/// <c>parameters.&lt;resource&gt;:delete</c> and the aggregate
/// <c>parameters:write</c> flag.
///
/// <see cref="PermissionSet.Contains"/> is used throughout (not
/// <see cref="PermissionSet.IsSatisfiedFor"/>): nav visibility only cares whether a
/// role holds a grant for a permission at all — restricted or not — since the
/// actual scoping (own organisation, Vlimpers-managed, ...) is enforced again by
/// the real command handlers. The <paramref name="permissions"/> passed in must
/// therefore include restricted grants (i.e. be resolved via the config-aware
/// <see cref="RolePermissionMap.For(System.Collections.Generic.IEnumerable{Role},Configuration.IOrganisationRegistryConfiguration,Microsoft.Extensions.Logging.ILogger)"/>
/// overload) or Vlimpers/Decentraal will look like they have no access at all.
/// </summary>
public static class GlobalPermissionTranslator
{
    public static IEnumerable<string> Translate(PermissionSet permissions)
    {
        var result = new HashSet<string>();
        var hasBodies = false;
        var hasParametersWrite = false;

        if (permissions.Contains(Permission.CanCreateOrganisations) ||
            permissions.Contains(Permission.CanManageChildren))
            result.Add("org.organisations:create");

        if (permissions.Contains(Permission.CanManageBodies))
            result.Add("body.info:create");

        if (permissions.Contains(Permission.CanImport))
            result.Add("imports");

        if (permissions.Contains(Permission.PeopleWrite))
            result.Add("people:write");

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
                var sub = ToKebabCase(name["BodiesCanManage".Length..]);
                result.Add($"bodies.{sub}:read");
                result.Add($"bodies.{sub}:write");
                hasBodies = true;
                continue;
            }

            if (name == nameof(Permission.PeopleFunctionsRead))
            {
                result.Add("people.functions:read");
                continue;
            }

            if (name == nameof(Permission.PeopleCapacitiesRead))
            {
                result.Add("people.capacities:read");
                continue;
            }

            if (!name.StartsWith("Parameters"))
                continue;

            var (resource, operation) = SplitParametersPermission(name);
            if (operation is null)
                continue;

            result.Add($"parameters.{resource}:{operation}");
            hasParametersWrite = hasParametersWrite || operation is "write" or "delete";
        }

        if (hasBodies)
        {
            result.Add("bodies:read");
            result.Add("bodies:write");
        }

        if (hasParametersWrite)
            result.Add("parameters:write");

        return result;
    }

    /// <summary>
    /// Splits a <c>Parameters{Resource}{Write|Delete}</c> permission name into
    /// its kebab-case resource segment and operation. Returns a <c>null</c> operation
    /// when the name doesn't match the expected suffix (defensive; every current
    /// <c>Parameters*</c> member does).
    /// </summary>
    private static (string Resource, string? Operation) SplitParametersPermission(string name)
    {
        const string prefix = "Parameters";

        foreach (var suffix in new[] { "Write", "Delete" })
        {
            if (!name.EndsWith(suffix) || name.Length <= prefix.Length + suffix.Length)
                continue;

            var resource = ToKebabCase(name[prefix.Length..^suffix.Length]);
            return (resource, suffix.ToLowerInvariant());
        }

        return (string.Empty, null);
    }

    /// <summary>
    /// Converts a PascalCase permission-name fragment (e.g. <c>OrganisationClassificationTypes</c>)
    /// into its kebab-case front-end resource name (<c>organisation-classification-types</c>).
    /// </summary>
    private static string ToKebabCase(string pascalCase)
    {
        var builder = new StringBuilder(pascalCase.Length + 8);

        for (var i = 0; i < pascalCase.Length; i++)
        {
            var c = pascalCase[i];

            if (char.IsUpper(c) && i > 0)
                builder.Append('-');

            builder.Append(char.ToLowerInvariant(c));
        }

        return builder.ToString();
    }
}

