namespace OrganisationRegistry.ArchitectureTests;

using System.Collections.Generic;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: huidige migratiestand vastpinnen — faalt bij regressie én bij vooruitgang (baseline bijwerken = ratchet).</summary>
public class MigrationProgressTests : ArchitectureTestBase
{
    private const string AuthorizeAttributeFullName =
        "OrganisationRegistry.Api.Infrastructure.Security.OrganisationRegistryAuthorizeAttribute";

    // ── Baselines (update DOWNWARD only) ──────────────────────────────────────

    /// <summary>Role-based [OrganisationRegistryAuthorize(Role...)] call sites remaining.</summary>
    private const int RoleAttributeUsageBaseline = 1;

    /// <summary>ISecurityPolicy implementations still referencing the Role enum.</summary>
    private const int RoleBasedPoliciesBaseline = 5;

    /// <summary>Non-edge types still referencing the Role enum.</summary>
    private const int RoleReferencesOutsideEdgeBaseline = 23;

    /// <summary>Controllers calling RoleMapping outside the edge translation layer.</summary>
    private const int InlineRoleMappingBaseline = 3;

    // ── Migrated state (update UPWARD only) ───────────────────────────────────

    /// <summary>Controllers already using RequiredPermissions.</summary>
    private const int PermissionMigratedControllersBaseline = 61;

    [Fact]
    public void RoleBasedAuthorizeAttributeUsageDoesNotGrow()
    {
        // [OrganisationRegistryAuthorize(Role.X, ...)] — role overload, marked Obsolete.
        var usageSites = Architecture.Types
            .OfType<IHasAttributes>()
            .SelectMany(t => t.AttributeInstances)
            .Concat(Architecture.Types.SelectMany(t => t.Members).SelectMany(m => m.AttributeInstances))
            .Where(a => a.Type.FullName == AuthorizeAttributeFullName)
            .Count(HasRoleArguments);

        AssertProgress("role-based authorize attribute usage", RoleAttributeUsageBaseline, usageSites);
    }

    [Fact]
    public void RoleBasedPoliciesDoNotGrow()
    {
        var policies = Classes().That()
            .ImplementInterface(typeof(Handling.Authorization.ISecurityPolicy))
            .GetObjects(Architecture);

        var roleType = Architecture.GetITypeOfType(typeof(Infrastructure.Authorization.Role));
        var count = policies.Count(p =>
            p.Dependencies.Any(d => d.Target.Equals(roleType))
            || p.DependenciesIncludingInherited.Any(d => d.Target.Equals(roleType)));

        AssertProgress("role-based policies", RoleBasedPoliciesBaseline, count);
    }

    [Fact]
    public void RoleReferencesOutsideTheEdgeLayerDoNotGrow()
    {
        var roleType = Architecture.GetITypeOfType(typeof(Infrastructure.Authorization.Role));
        var edgeNamespaces = new[]
        {
            "OrganisationRegistry.Infrastructure.Authorization",
            "OrganisationRegistry.Api.Security",
            "OrganisationRegistry.Api.Infrastructure.Security",
        };

        var count = Architecture.Types
            .Where(t => t.FullName != typeof(Infrastructure.Authorization.Role).FullName)
            .Where(t => !edgeNamespaces.Any(ns => t.Namespace?.FullName?.StartsWith(ns) == true))
            .Count(t => t.Dependencies.Any(d => d.Target.Equals(roleType)));

        AssertProgress("Role references outside the edge layer", RoleReferencesOutsideEdgeBaseline, count);
    }

    [Fact]
    public void InlineRoleMappingUsageDoesNotGrow()
    {
        var roleMapping = Architecture.GetITypeOfType(typeof(Api.Security.RoleMapping));
        var edgeNamespaces = new[]
        {
            "OrganisationRegistry.Api.Security",
            "OrganisationRegistry.Api.Infrastructure.Security",
        };

        var count = Architecture.Classes
            .Where(c => !edgeNamespaces.Any(ns => c.Namespace?.FullName?.StartsWith(ns) == true))
            .Count(c => c.Dependencies.Any(d => d.Target.Equals(roleMapping))
                        || c.DependenciesIncludingInherited.Any(d => d.Target.Equals(roleMapping)));

        AssertProgress("inline RoleMapping usages outside the edge layer", InlineRoleMappingBaseline, count);
    }

    [Fact]
    public void PermissionMigratedControllersDoNotShrink()
    {
        var controllers = Classes().That().AreAssignableTo(typeof(Controller))
            .And().AreNotAbstract()
            .GetObjects(Architecture);

        var count = controllers.Count(c =>
            c.AttributeInstances.Any(a => a.Type.FullName == AuthorizeAttributeFullName && HasRequiredPermissions(a))
            || c.Members.OfType<MethodMember>()
                .SelectMany(m => m.AttributeInstances)
                .Any(a => a.Type.FullName == AuthorizeAttributeFullName && HasRequiredPermissions(a)));

        AssertInverseProgress("controllers migrated to RequiredPermissions", PermissionMigratedControllersBaseline, count);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Remaining-work counter: fails when the count grows (regression) or shrinks
    /// (migration progressed → lower the baseline to lock in the improvement).
    /// </summary>
    private static void AssertProgress(string description, int baseline, int actual)
    {
        Assert.True(
            actual == baseline,
            $"Migration regression/progress on '{description}': baseline {baseline}, actual {actual}. " +
            (actual > baseline
                ? "REGRESSION: new role-based code was introduced."
                : $"PROGRESS: lower the baseline to {actual} to lock in the improvement."));
    }

    /// <summary>
    /// Migrated-work counter: fails when the count shrinks (regression) or grows
    /// (migration progressed → raise the baseline).
    /// </summary>
    private static void AssertInverseProgress(string description, int baseline, int actual)
    {
        Assert.True(
            actual == baseline,
            $"Migration regression/progress on '{description}': baseline {baseline}, actual {actual}. " +
            (actual < baseline
                ? "REGRESSION: permission-migrated code was removed or reverted."
                : $"PROGRESS: raise the baseline to {actual} to lock in the improvement."));
    }

    private static bool HasRoleArguments(AttributeInstance attribute)
        // The role overload takes params Role[] as a positional constructor argument.
        // Permission-based usage sets RequiredPermissions as a NAMED argument instead.
        => attribute.AttributeArguments.Any(arg =>
            arg is not AttributeNamedArgument
            && arg.Value is object[] { Length: > 0 });

    private static bool HasRequiredPermissions(AttributeInstance attribute)
        => attribute.AttributeArguments.Any(arg =>
            arg is AttributeNamedArgument named
            && named.Name == "RequiredPermissions"
            && named.Value is object[] { Length: > 0 });
}
