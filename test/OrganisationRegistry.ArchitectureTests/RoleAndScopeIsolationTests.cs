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

/// <summary>Waarom: rollen mogen na vertaling aan de rand nergens meer voorkomen — enkel in de vertaaltabellen (SC-006).</summary>
public partial class RoleAndScopeIsolationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<IType> RoleEnum =
        Types().That().Are(typeof(Infrastructure.Authorization.Role)).As("Role enum");

    private static readonly IObjectProvider<IType> EdgeTranslationLayer =
        Types().That()
            .ResideInNamespace("OrganisationRegistry.Infrastructure.Authorization")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Security")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Infrastructure.Security")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Auth")
            .Or().ResideInNamespace("OrganisationRegistry.Api.Auth.Models")
            .As("edge translation layer");

    [Fact]
    public void RoleEnumIsOnlyReferencedFromTheEdgeTranslationLayer()
    {
        IArchRule rule = Types().That().AreNot(RoleEnum)
            .And().AreNot(EdgeTranslationLayer)
            .Should().NotDependOnAny(RoleEnum)
            .Because("roles must be translated to permissions at the edge (FR-015, SC-006)");

        rule.Check(Architecture);
    }

    /// <summary>
    /// Waarom: ArchUnitNET type dependencies missen veel directe Role enum
    /// gebruiken (b.v. securityInfo.Roles.Contains(Role.X)). Een source-scan
    /// is nodig om SC-006 strikt af te dwingen buiten de edge layer.
    /// </summary>
    [Fact]
    public void RoleEnumValuesAreNotReferencedOutsideTheEdgeTranslationLayer()
    {
        var repoRoot = GetRepositoryRoot();
        var violations = new List<string>();

        foreach (var file in EnumerateSourceFiles(repoRoot))
        {
            if (IsEdgeTranslationFile(file))
                continue;

            if (IsTestFile(file))
                continue;

            var lines = File.ReadAllLines(file);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                if (line.TrimStart().StartsWith("//"))
                    continue;

                // Match whole-word Role. usage (e.g. Role.AlgemeenBeheerder)
                if (RoleReferencePattern().IsMatch(line))
                    violations.Add($"{MakeRelative(repoRoot, file)}:{i + 1}: {line.Trim()}");
            }
        }

        if (violations.Any())
        {
            var message = "Role enum values referenced outside edge translation layer:" + Environment.NewLine +
                          string.Join(Environment.NewLine, violations);
            Assert.Fail(message);
        }
    }

    private static string GetRepositoryRoot()
    {
        var current = AppContext.BaseDirectory;
        while (current != null && !File.Exists(Path.Combine(current, "OrganisationRegistry.sln")))
            current = Directory.GetParent(current)?.FullName;

        return current ?? throw new InvalidOperationException("Could not locate repository root");
    }

    private static IEnumerable<string> EnumerateSourceFiles(string repoRoot)
    {
        foreach (var dir in new[] { "src", "test" })
        {
            var path = Path.Combine(repoRoot, dir);
            if (!Directory.Exists(path))
                continue;

            foreach (var file in Directory.EnumerateFiles(path, "*.cs", SearchOption.AllDirectories))
            {
                if (file.EndsWith(".g.cs"))
                    continue;

                yield return file;
            }
        }
    }

    private static bool IsEdgeTranslationFile(string file)
    {
        var relative = file.Replace('\\', '/');

        var edgeFiles = new[]
        {
            "src/OrganisationRegistry.Infrastructure/Authorization/Role.cs",
            "src/OrganisationRegistry.Infrastructure/Authorization/RoleMapping.cs",
            "src/OrganisationRegistry.Infrastructure/Authorization/RolePermissionMap.cs",
            "src/OrganisationRegistry.Infrastructure/Authorization/ScopePermissionMap.cs",
            "src/OrganisationRegistry.Api/Auth/Models/RolePermissions.cs",
            "src/OrganisationRegistry.Api/Auth/Models/RolePriority.cs",
            "src/OrganisationRegistry.Api/Auth/Models/RoleDisplayNames.cs",
            "src/OrganisationRegistry.Api/Security/OrganisationRegistryTokenBuilder.cs",
            "src/OrganisationRegistry.Api/Security/TokenExchangeClaimsTransformation.cs",
            "src/OrganisationRegistry.Api/Security/RoleMapping.cs",
            "src/OrganisationRegistry.Api/Infrastructure/Security/OrganisationRegistryAuthorizeAttribute.cs",
        };

        return edgeFiles.Any(edge => relative.EndsWith(edge, StringComparison.OrdinalIgnoreCase));
    }

    private static bool IsTestFile(string file)
        => file.Replace('\\', '/').Contains("/test/", StringComparison.OrdinalIgnoreCase);

    private static string MakeRelative(string repoRoot, string file)
        => file[(repoRoot.Length + 1)..].Replace('\\', '/');

    [GeneratedRegex(@"\bRole\.[A-Za-z_][A-Za-z0-9_]*\b")]
    private static partial Regex RoleReferencePattern();
}
