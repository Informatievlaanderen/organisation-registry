namespace OrganisationRegistry.ArchitectureTests;

using System;
using System.Linq;
using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.xUnit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using static ArchUnitNET.Fluent.ArchRuleDefinition;

/// <summary>Waarom: elke controller maakt een expliciete beveiligingskeuze; elke mutatie vereist een permissie (FR-006).</summary>
public class ControllerAuthorizationTests : ArchitectureTestBase
{
    private static readonly IObjectProvider<Class> ConcreteControllers =
        Classes().That().AreAssignableTo(typeof(Controller))
            .And().AreNotAbstract()
            .As("concrete controllers");

    private const string AuthorizeAttributeFullName =
        "OrganisationRegistry.Api.Infrastructure.Security.OrganisationRegistryAuthorizeAttribute";

    private static readonly string AnonymousAttributeFullName = typeof(AllowAnonymousAttribute).FullName;

    [Fact]
    public void AllowAnonymousOnlyAppearsTogetherWithOrganisationRegistryAuthorize()
    {
        // [AllowAnonymous] neutraliseert de BackofficeUser-policy. Enige geldige gebruik
        // is de combinatie met [OrganisationRegistryAuthorize]: policy-check valt weg,
        // maar de user wordt nog steeds opgehaald als die er is (publieke leeslijsten
        // met user-afhankelijke verrijking — zie commits f2934f8c5 en c7f7538ae).
        // Kaal [AllowAnonymous] = guard weg én geen user-context — anti-patroon.
        var controllers = ConcreteControllers.GetObjects(Architecture);
        var violations = controllers
            .SelectMany(c =>
            {
                var classTargets = c.AttributeInstances
                    .Any(a => a.Type.FullName == AnonymousAttributeFullName)
                    ? new[]
                    {
                        (Target: c.FullName,
                         HasAuthorize: c.AttributeInstances.Any(a => a.Type.FullName == AuthorizeAttributeFullName))
                    }
                    : Array.Empty<(string Target, bool HasAuthorize)>();

                var methodTargets = c.Members
                    .OfType<MethodMember>()
                    .Where(m => m.AttributeInstances.Any(a => a.Type.FullName == AnonymousAttributeFullName))
                    .Select(m => (Target: $"{c.FullName}.{m.Name}",
                        HasAuthorize: m.AttributeInstances.Any(a => a.Type.FullName == AuthorizeAttributeFullName)));

                return classTargets.Concat(methodTargets)
                    .Where(t => !t.HasAuthorize)
                    .Select(t => t.Target);
            })
            .OrderBy(name => name)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "[AllowAnonymous] is only allowed when [OrganisationRegistryAuthorize] is " +
            "already present on the same controller or method.\n" +
            "Bare [AllowAnonymous] on:\n - " + string.Join("\n - ", violations));
    }

    [Fact]
    public void MutatingActionsRequirePermissionOnActionOrController()
    {
        // POST/PUT/DELETE/PATCH actions must be covered by an
        // [OrganisationRegistryAuthorize] with RequiredPermissions set, either on the
        // action itself or on the declaring controller.
        var httpMethodAttributes = new[]
        {
            typeof(HttpPostAttribute).FullName,
            typeof(HttpPutAttribute).FullName,
            typeof(HttpDeleteAttribute).FullName,
            typeof(HttpPatchAttribute).FullName,
        };

        var controllers = ConcreteControllers.GetObjects(Architecture);
        var violations = controllers
            .SelectMany(c =>
            {
                var classHasAuthorizeWithPermissions = c.AttributeInstances
                    .Where(a => a.Type.FullName == AuthorizeAttributeFullName)
                    .Any(HasRequiredPermissions);

                return c.Members
                    .OfType<MethodMember>()
                    .Where(m => m.AttributeInstances.Any(a => httpMethodAttributes.Contains(a.Type.FullName)))
                    .Where(m => !m.AttributeInstances.Any(a => a.Type.FullName == AnonymousAttributeFullName))
                    .Where(m =>
                    {
                        var methodHasAuthorizeWithPermissions = m.AttributeInstances
                            .Where(a => a.Type.FullName == AuthorizeAttributeFullName)
                            .Any(HasRequiredPermissions);

                        return !methodHasAuthorizeWithPermissions && !classHasAuthorizeWithPermissions;
                    })
                    .Select(m => $"{c.FullName}.{m.Name}");
            })
            .OrderBy(name => name)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "Every mutating action (POST/PUT/DELETE/PATCH) must require a permission via " +
            "[OrganisationRegistryAuthorize(RequiredPermissions = ...)] on the action or controller (FR-006).\n" +
            "Unprotected mutating actions:\n - " + string.Join("\n - ", violations));
    }

    [Fact]
    public void ParameterGetActionsAllowBothAnonymousAndAuthenticatedAccess()
    {
        // Parameter GETs (referentiedata-lijsten) moeten publiek leesbaar zijn.
        // Ofwel op controller-niveau, ofwel op elke GET-method: [AllowAnonymous] +
        // [OrganisationRegistryAuthorize] samen — anoniem leesbaar, user beschikbaar
        // als die er is.
        var controllers = ConcreteControllers.GetObjects(Architecture);
        var violations = controllers
            .Where(c => c.Namespace?.FullName?.Contains("Parameters") == true)
            .SelectMany(c =>
            {
                var classHasCombo = c.AttributeInstances.Any(a => a.Type.FullName == AnonymousAttributeFullName)
                    && c.AttributeInstances.Any(a => a.Type.FullName == AuthorizeAttributeFullName);

                return c.Members
                    .OfType<MethodMember>()
                    .Where(m => m.AttributeInstances.Any(a => a.Type.FullName == typeof(HttpGetAttribute).FullName))
                    .Where(m =>
                    {
                        if (classHasCombo) return false;
                        var methodHasCombo = m.AttributeInstances.Any(a => a.Type.FullName == AnonymousAttributeFullName)
                            && m.AttributeInstances.Any(a => a.Type.FullName == AuthorizeAttributeFullName);
                        return !methodHasCombo;
                    })
                    .Select(m => $"{c.FullName}.{m.Name}");
            })
            .OrderBy(name => name)
            .ToList();

        Assert.True(
            violations.Count == 0,
            "Parameter GET actions must allow both anonymous and authenticated access: [AllowAnonymous] + " +
            "[OrganisationRegistryAuthorize] together, on the class or each method.\n" +
            "Missing the combo on:\n - " + string.Join("\n - ", violations));
    }

    private static bool HasRequiredPermissions(AttributeInstance attribute)
        // RequiredPermissions is a settable property → named attribute argument whose
        // value is a non-empty Permission[] (array of enum constants).
        => attribute.AttributeArguments.Any(arg =>
            arg is AttributeNamedArgument named
            && named.Name == "RequiredPermissions"
            && named.Value is object[] { Length: > 0 });
}
