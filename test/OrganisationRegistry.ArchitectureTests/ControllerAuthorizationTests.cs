namespace OrganisationRegistry.ArchitectureTests;

using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Domain.Extensions;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Infrastructure.Authorization;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: controllers drukken autorisatie uit als permissies, en parameter-GETs zijn publiek (FR-006, FR-007).</summary>
public class ControllerAuthorizationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<Class> ControllerClasses =
        Classes().That()
            .AreAssignableTo(typeof(Api.Infrastructure.OrganisationRegistryController))
            .As("controllers");

    private static bool HasAttribute(IHasAttributes target, string attributeName)
        => target.AttributeInstances.Any(a => a.Type.Name == attributeName);

    private static bool HasRequiredPermissions(IHasAttributes target)
        => target.AttributeInstances
            .Where(a => a.Type.Name == "OrganisationRegistryAuthorizeAttribute")
            .SelectMany(a => a.AttributeArguments)
            .Any(arg =>
            {
                if (arg is not ArchUnitNET.Domain.AttributeNamedArgument named) return false;
                return named.Name == "RequiredPermissions";
            });

    [Fact]
    public void ParameterGetActionsAllowBothAnonymousAndAuthenticatedAccess()
    {
        var parameterControllers = ControllerClasses.GetObjects(Architecture)
            .Where(c => c.Name.Contains("Parameter") || c.Name == "ParameterController");

        var violations = new System.Collections.Generic.List<string>();

        foreach (var controller in parameterControllers)
        {
            bool classAllowsAnonymous = HasAttribute(controller, "AllowAnonymousAttribute");
            bool classAuthorizes = HasAttribute(controller, "OrganisationRegistryAuthorizeAttribute");

            var getMethods = controller.Members.OfType<MethodMember>()
                .Where(m => m.Name == "Get");

            foreach (var method in getMethods)
            {
                bool methodAllowsAnonymous = HasAttribute(method, "AllowAnonymousAttribute");
                bool methodAuthorizes = HasAttribute(method, "OrganisationRegistryAuthorizeAttribute");

                bool anonymousAllowed = classAllowsAnonymous || methodAllowsAnonymous;
                bool authenticatedAllowed = classAuthorizes || methodAuthorizes;

                if (!anonymousAllowed || !authenticatedAllowed)
                    violations.Add($"{controller.FullName}.{method.Name} does not allow both anonymous and authenticated access");
            }
        }

        Assert.Empty(violations);
    }

    [Fact]
    public void MutatingActionsRequirePermissionOnActionOrController()
    {
        var controllers = ControllerClasses.GetObjects(Architecture);
        var violations = new System.Collections.Generic.List<string>();

        foreach (var controller in controllers)
        {
            bool controllerHasPermissions = HasRequiredPermissions(controller);

            var mutating = controller.Members.OfType<MethodMember>()
                .Where(m =>
                {
                    var httpAttrs = new[] { "HttpPostAttribute", "HttpPutAttribute", "HttpPatchAttribute", "HttpDeleteAttribute" };
                    return httpAttrs.Any(a => m.AttributeInstances.Any(attr => attr.Type.Name == a));
                });

            foreach (var method in mutating)
            {
                bool methodHasPermissions = HasRequiredPermissions(method);
                bool methodAllowsAnonymous = HasAttribute(method, "AllowAnonymousAttribute");
                bool methodAuthorizes = HasAttribute(method, "OrganisationRegistryAuthorizeAttribute");
                bool isPublicMutating = methodAllowsAnonymous && methodAuthorizes;

                if (!isPublicMutating && !controllerHasPermissions && !methodHasPermissions)
                    violations.Add($"{controller.FullName}.{method.Name} has no RequiredPermissions");
            }
        }

        Assert.Empty(violations);
    }
}
