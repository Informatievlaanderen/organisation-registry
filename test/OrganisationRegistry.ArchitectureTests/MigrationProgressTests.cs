namespace OrganisationRegistry.ArchitectureTests;

using System;
using System.Collections.Generic;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: ratchets die afdwingen dat de migratie niet terugdraait en progressie zichtbaar maken.</summary>
public class MigrationProgressTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<Class> ControllerClasses =
        Classes().That()
            .AreAssignableTo(typeof(Api.Infrastructure.OrganisationRegistryController))
            .As("controllers");

    private static readonly string RoleAttributeName = "OrganisationRegistryAuthorizeAttribute";

    private static int CountRoleBasedControllers()
    {
        return ControllerClasses.GetObjects(Architecture)
            .Count(c => c.AttributeInstances.Any(a => a.Type.Name == RoleAttributeName &&
                                              a.AttributeArguments.Any(arg =>
                                                  arg is ArchUnitNET.Domain.AttributeNamedArgument named &&
                                                  named.Name == "Role")));
    }

    private static int CountPermissionMigratedControllers()
    {
        return ControllerClasses.GetObjects(Architecture)
            .Count(c =>
            {
                bool controllerHasPermissions = c.AttributeInstances.Any(a => a.Type.Name == RoleAttributeName &&
                    a.AttributeArguments.Any(arg =>
                        arg is ArchUnitNET.Domain.AttributeNamedArgument named && named.Name == "RequiredPermissions"));

                bool methodHasPermissions = c.Members.OfType<MethodMember>().Any(m =>
                    m.AttributeInstances.Any(a => a.Type.Name == RoleAttributeName &&
                        a.AttributeArguments.Any(arg =>
                            arg is ArchUnitNET.Domain.AttributeNamedArgument named && named.Name == "RequiredPermissions")));

                return controllerHasPermissions || methodHasPermissions;
            });
    }

    [Fact]
    public void RoleBasedControllersDoNotGrow()
    {
        const int baseline = 1;
        var actual = CountRoleBasedControllers();
        Assert.True(actual <= baseline, $"Role-based controller count regression: baseline {baseline}, actual {actual}.");
    }

    [Fact]
    public void PermissionMigratedControllersDoNotShrink()
    {
        const int baseline = 63;
        var actual = CountPermissionMigratedControllers();
        Assert.True(actual >= baseline, $"Permission-migrated controller count regression: baseline {baseline}, actual {actual}. If this is progress, raise the baseline.");
    }
}
