namespace OrganisationRegistry.Api.Auth.Models;

using System.Collections.Generic;
using System.Text;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Translates the fine-grained internal <see cref="Permission"/> grants for a role
/// (as defined in <see cref="RolePermissionMap"/>) into the "&lt;resource&gt;:&lt;operation&gt;"
/// strings the front-end uses to decide which nav tabs and screens to show
/// (e.g. "parameters:read", "parameters.locations:write").
///
/// This is driven directly by <see cref="RolePermissionMap"/> — the actual source of
/// truth for what a role may do — instead of a second, hand-maintained list, so new
/// <c>Parameters*</c> permissions granted there automatically show up on <c>/v1/me</c>
/// without needing a matching manual edit here.
///
/// The <c>Parameters*Write/Delete</c>, <c>CanManageBodies</c>, <c>People*</c> and
/// <c>System</c> families are translated, as well as the top-level "can this role
/// create/manage organisations or bodies at all" flags (<c>organisations:create</c>,
/// <c>bodies:create</c>), <c>imports</c> and <c>delegations:read</c>/
/// <c>delegations:write</c>/<c>delegations:delete</c>
/// (<see cref="Permission.DelegationsCreate"/> is deliberately not translated
/// — it is an internal-only, Developer-only capability with no corresponding
/// rights-table entry).
/// Per-organisation permissions (e.g. <c>CanManageKeys</c>, <c>CanManageCapacities</c>)
/// are scoped to an individual organisation's detail page and are not relevant to
/// global nav visibility, so they are intentionally not translated here.
///
/// There is no <c>Parameters*Read</c> permission: reading a master-data list is
/// open to any authenticated backoffice user (see the parameter list
/// controllers), so it carries no per-role signal for nav visibility. Only the
/// write/delete grants (which remain AlgemeenBeheerder/Developer-only today)
/// are translated into <c>parameters.&lt;resource&gt;:write</c> and
/// <c>parameters.&lt;resource&gt;:delete</c>.
///
/// <see cref="Permission.System"/> is a single, unsplit permission covering
/// three admin screens (Statistieken, Events, Stopgezet in KBO — see
/// <c>ui-permission-matrix.md</c>); holding it expands into all three
/// <c>system.&lt;screen&gt;:read</c> strings plus the <c>system</c> aggregate.
///
/// Holding at least one permission in the <c>Parameters*</c>, <c>People*</c>,
/// <c>Delegations*</c>, <c>Bodies*</c>/<c>CanManageBodies</c> or
/// <see cref="Permission.System"/> family additionally surfaces the bare
/// aggregate flag for that family (<c>parameters</c>, <c>people</c>,
/// <c>delegations</c>, <c>bodies</c>, <c>system</c>) so the front-end can show
/// the top-level nav entry without inspecting every sub-permission.
/// For bodies, only the aggregate flag and the create flag are surfaced: the
/// front-end only needs to know whether the Bodies tab should be visible and
/// whether a user can create a body.
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
        var hasParameters = false;
        var hasPeople = false;
        var hasDelegations = false;

        if (permissions.Contains(Permission.CanCreateOrganisations) ||
            permissions.Contains(Permission.CanManageChildren))
            result.Add("organisations:create");

        if (permissions.Contains(Permission.CanManageBodies))
        {
            result.Add("bodies:create");
            hasBodies = true;
        }

        if (permissions.Contains(Permission.CanImport))
            result.Add("imports");

        if (permissions.Contains(Permission.PeopleWrite))
        {
            result.Add("people:write");
            hasPeople = true;
        }

        if (permissions.Contains(Permission.DelegationsRead))
        {
            result.Add("delegations:read");
            hasDelegations = true;
        }

        if (permissions.Contains(Permission.DelegationsWrite))
        {
            result.Add("delegations:write");
            hasDelegations = true;
        }

        if (permissions.Contains(Permission.DelegationsDelete))
        {
            result.Add("delegations:delete");
            hasDelegations = true;
        }

        if (permissions.Contains(Permission.System))
        {
            result.Add("system.statistics:read");
            result.Add("system.events:read");
            result.Add("system.kbo-terminated:read");
            result.Add("system");
        }

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
                hasBodies = true;
                continue;
            }

            if (name == nameof(Permission.PeopleFunctionsRead))
            {
                result.Add("people.functions:read");
                hasPeople = true;
                continue;
            }

            if (name == nameof(Permission.PeopleCapacitiesRead))
            {
                result.Add("people.capacities:read");
                hasPeople = true;
                continue;
            }

            if (!name.StartsWith("Parameters"))
                continue;

            var (resource, operation) = SplitParametersPermission(name);
            if (operation is null)
                continue;

            result.Add($"parameters.{resource}:{operation}");
            hasParameters = true;
        }

        if (hasBodies)
            result.Add("bodies");

        if (hasParameters)
            result.Add("parameters");

        if (hasPeople)
            result.Add("people");

        if (hasDelegations)
            result.Add("delegations");

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

