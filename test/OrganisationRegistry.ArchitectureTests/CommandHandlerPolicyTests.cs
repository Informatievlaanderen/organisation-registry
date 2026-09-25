namespace OrganisationRegistry.ArchitectureTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: command handlers moeten policies gebruiken; policies moeten permission-based zijn (geen rollen).</summary>
public class CommandHandlerPolicyTests : ArchitectureTestBase
{
    private static readonly string DomainRoot = FindDomainRoot();

    private const int HandlersWithPolicyBaseline = 67;
    private const int HandlersWithoutPolicyBaseline = 34;

    private static readonly string[] PolicyCallPatterns =
    {
        "RequiresPermission", "RequiresVlimpersBeheerder", "RequiresDecentraleBeheerder",
        "RequiresRoles", "RequiresRegisterBody",
        ".WithPolicy(", ".Check(",
        "WithBodyPolicy", "WithImportPolicy", "WithChildPolicy", "WithParentPolicy",
        "WithKboPolicy", "WithRegulationPolicy",
        "WithVlimpersManagementPolicy",
        "RequiresBeheerderForOrganisationButNotUnderVlimpersManagement",
        "RequiresBeheerderForOrganisationLimitedToVlimpers",
        "RequiresBeheerderForOrganisationNotLimitedToVlimpers",
    };

    private static readonly string[] PermissionMethods =
    {
        "IsSatisfiedFor(", "HasPermission(", "HasAnyPermission(",
    };

    [Fact]
    public void EveryPolicyUsesPermissionChecksNeverRoles()
    {
        // Alle ISecurityPolicy-implementaties moeten permissions gebruiken
        // (HasPermission / IsSatisfiedFor), niet rollen (IsInAnyOf).
        var policies = Classes().That()
            .ImplementInterface(typeof(Handling.Authorization.ISecurityPolicy))
            .GetObjects(Architecture)
            .ToList();

        var violations = policies
            .Where(p => p.FullName != "OrganisationRegistry.Handling.Authorization.RequiresRolesPolicy")
            .Where(p =>
            {
                var source = ReadSource(p.FullName);
                return source != null
                    && source.Contains("IsInAnyOf")
                    && !PermissionMethods.Any(m => source.Contains(m));
            })
            .Select(p => p.FullName)
            .OrderBy(name => name)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "Every ISecurityPolicy must use permission checks (HasPermission/IsSatisfiedFor), " +
            "never role checks (IsInAnyOf).\n" +
            "Role-based policies:\n - " + string.Join("\n - ", violations));
    }

    [Fact]
    public void CommandHandlerPolicyCoverageDoesNotShrink()
    {
        var handlers = FindCommandHandlerFiles();
        var (withPolicy, withoutPolicy) = PartitionByPolicyUsage(handlers);

        Assert.True(
            withPolicy.Count == HandlersWithPolicyBaseline,
            $"Command handlers with policy: baseline {HandlersWithPolicyBaseline}, actual {withPolicy.Count}. " +
            (withPolicy.Count < HandlersWithPolicyBaseline
                ? "REGRESSION: a policy was removed."
                : $"PROGRESS: raise the baseline to {withPolicy.Count} to lock in."));
    }

    [Fact]
    public void CommandHandlerWithoutPolicyCountDoesNotGrow()
    {
        var handlers = FindCommandHandlerFiles();
        var (withPolicy, withoutPolicy) = PartitionByPolicyUsage(handlers);

        Assert.True(
            withoutPolicy.Count == HandlersWithoutPolicyBaseline,
            $"Command handlers without policy: baseline {HandlersWithoutPolicyBaseline}, actual {withoutPolicy.Count}. " +
            (withoutPolicy.Count > HandlersWithoutPolicyBaseline
                ? "REGRESSION: new handlers were added without a policy."
                : $"PROGRESS: lower the baseline to {withoutPolicy.Count} to lock in."));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static (List<string> With, List<string> Without) PartitionByPolicyUsage(List<string> handlerFiles)
    {
        var with = new List<string>();
        var without = new List<string>();

        foreach (var file in handlerFiles)
        {
            var source = File.ReadAllText(file);
            if (PolicyCallPatterns.Any(p => source.Contains(p, StringComparison.Ordinal)))
                with.Add(file);
            else
                without.Add(file);
        }

        return (with, without);
    }

    private static List<string> FindCommandHandlerFiles()
    {
        var domainSrc = Path.Combine(DomainRoot, "src", "OrganisationRegistry");
        return Directory.EnumerateFiles(domainSrc, "*CommandHandler*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(Path.Combine("bin", ""))
                        && !f.Contains(Path.Combine("obj", ""))
                        && !f.Contains(Path.Combine("Handling", "")))
            .Where(f =>
            {
                var content = File.ReadAllText(f);
                return Regex.IsMatch(content, @"public\s+.*\s+Handle\s*\(\s*ICommandEnvelope");
            })
            .OrderBy(f => f)
            .ToList();
    }

    private static string? ReadSource(string fullTypeName)
    {
        // Map OrganisationRegistry.Handling.Authorization.FooPolicy → FooPolicy.cs
        var parts = fullTypeName.Split('.');
        var className = parts[^1];
        var domainSrc = Path.Combine(DomainRoot, "src", "OrganisationRegistry");
        var candidates = Directory.EnumerateFiles(domainSrc, $"{className}.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(Path.Combine("bin", ""))
                        && !f.Contains(Path.Combine("obj", "")));
        var file = candidates.FirstOrDefault();
        return file != null ? File.ReadAllText(file) : null;
    }

    private static string FindDomainRoot()
    {
        var dir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)!;
        while (dir != null && !File.Exists(Path.Combine(dir, "OrganisationRegistry.sln")))
            dir = Directory.GetParent(dir)?.FullName;
        return dir ?? throw new InvalidOperationException("Could not find solution root");
    }
}
