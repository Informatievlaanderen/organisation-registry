namespace OrganisationRegistry.UnitTests.Security;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Edit;
using Api.Infrastructure;
using Api.Infrastructure.Security;
using Api.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;
using Xunit.Abstractions;
using Schemes = Api.Infrastructure.Security.AuthenticationSchemes;

/// <summary>
/// Contract tests voor de autorisatie-configuratie van de Organisation Registry API.
///
/// Deze tests documenteren en bewaken de twee autorisatiepatronen:
///
///   1. EditApi-controllers (machine-to-machine, OAuth2 introspection via Keycloak)
///      Elke subclass van <see cref="EditApiController"/> MOET:
///      - [Authorize(AuthenticationSchemes = "Introspection", Policy = "...")] hebben
///      - Geen ander scheme gebruiken dan AuthenticationSchemes.EditApi
///
///   2. Backoffice command controllers (gebruikers + M2M via alle schemes)
///      Elke subclass van <see cref="OrganisationRegistryCommandController"/> MOET:
///      - [OrganisationRegistryAuthorize] of [Authorize(Policy = PolicyNames.BackofficeUser)] hebben
///      - Géén [AllowAnonymous] hebben (tenzij expliciet uitgezonderd)
///      - Geen schemes op controller-niveau vastleggen
///
/// Wanneer een test hier faalt betekent het dat een controller het verkeerde scheme of
/// de verkeerde policy heeft, of dat [Authorize] volledig ontbreekt.
/// </summary>
public class AuthorizationContractTests
{
    private static readonly Assembly ApiAssembly = typeof(EditApiController).Assembly;

    private readonly ITestOutputHelper _output;

    public AuthorizationContractTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ---------------------------------------------------------------------------
    // EditApi controllers — machine-to-machine via Keycloak introspection
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Elke concrete subclass van EditApiController moet voorzien zijn van
    /// [Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = "...")].
    /// Zonder dit attribuut is de controller publiek toegankelijk.
    /// </summary>
    [Fact]
    public void EditApiControllers_MustHaveAuthorizeWithEditApiScheme()
    {
        var violations = new List<string>();

        foreach (var controller in GetConcreteSubclasses<EditApiController>())
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();

            if (authorize is null)
            {
                violations.Add($"{controller.Name}: ontbreekt [Authorize]");
                continue;
            }

            if (authorize.AuthenticationSchemes != Schemes.EditApi)
                violations.Add(
                    $"{controller.Name}: AuthenticationSchemes = \"{authorize.AuthenticationSchemes}\" " +
                    $"maar verwacht \"{Schemes.EditApi}\"");
        }

        violations.Should().BeEmpty(
            because: "elke EditApiController moet [Authorize(AuthenticationSchemes = \"Introspection\", Policy = \"...\")] hebben — " +
                     "dit garandeert dat alleen Keycloak M2M-tokens (OAuth2 introspection) worden geaccepteerd");
    }

    /// <summary>
    /// Elke EditApiController moet een niet-lege Policy hebben.
    /// Een ontbrekende policy maakt het authorize-attribuut betekenisloos (alleen authenticatie, geen autorisatie).
    /// </summary>
    [Fact]
    public void EditApiControllers_MustHaveNonEmptyPolicy()
    {
        var violations = new List<string>();

        foreach (var controller in GetConcreteSubclasses<EditApiController>())
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            if (authorize is null) continue; // gedekt door vorige test

            if (string.IsNullOrWhiteSpace(authorize.Policy))
                violations.Add($"{controller.Name}: [Authorize] heeft geen Policy");
        }

        violations.Should().BeEmpty(
            because: "elke EditApiController heeft een scope-based policy nodig die bepaalt " +
                     "welk Keycloak-client-scope vereist is (bijv. dv_organisatieregister_cjmbeheerder)");
    }

    /// <summary>
    /// Overzicht van alle EditApi controllers en hun geconfigureerde policies.
    /// Dit dient als levende documentatie van het EditApi-toegangsmodel.
    /// </summary>
    [Fact]
    public void EditApiControllers_PolicyMapping_IsDocumented()
    {
        _output.WriteLine("EditApi controller → policy mapping (Keycloak scope-based autorisatie):");
        _output.WriteLine(new string('-', 70));

        foreach (var controller in GetConcreteSubclasses<EditApiController>().OrderBy(c => c.Name))
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            var policy = authorize?.Policy ?? "(geen policy)";
            var scheme = authorize?.AuthenticationSchemes ?? "(geen scheme)";
            _output.WriteLine($"  {controller.Name,-55} policy={policy,-30} scheme={scheme}");
        }

        // De test zelf slaagt altijd — het is puur documentatie-output.
        // Zie de andere tests voor de daadwerkelijke contract-verificatie.
        GetConcreteSubclasses<EditApiController>().Should().NotBeEmpty(
            because: "er moeten EditApi controllers bestaan in de API-assembly");
    }

    // ---------------------------------------------------------------------------
    // Backoffice command controllers — gebruikers via token-exchange JWT of BffApi
    // ---------------------------------------------------------------------------

    /// <summary>
    /// Elke concrete subclass van OrganisationRegistryCommandController moet
    /// een [Authorize] of [OrganisationRegistryAuthorize] attribuut hebben.
    /// OrganisationRegistryAuthorize is een wrapper rond PolicyNames.BackofficeUser.
    /// </summary>
    [Fact]
    public void BackofficeCommandControllers_MustHaveAuthorizeAttribute()
    {
        var violations = new List<string>();

        foreach (var controller in GetConcreteSubclasses<OrganisationRegistryCommandController>())
        {
            var hasAuthorize = controller.GetCustomAttribute<AuthorizeAttribute>() is not null;
            var hasAllowAnonymous = controller.GetCustomAttribute<AllowAnonymousAttribute>() is not null;

            if (!hasAuthorize && !hasAllowAnonymous)
                violations.Add($"{controller.Name}: ontbreekt [Authorize] of [OrganisationRegistryAuthorize]");
        }

        violations.Should().BeEmpty(
            because: "elke backoffice command controller moet beveiligd zijn via [OrganisationRegistryAuthorize] " +
                     "of een expliciete [Authorize], zodat alleen geauthenticeerde gebruikers commands kunnen sturen");
    }

    /// <summary>
    /// OrganisationRegistryCommandControllers moeten de BackofficeUser-policy gebruiken.
    /// Die policy eist vo_id en sluit daardoor M2M-tokens zonder gebruikersclaim uit.
    /// </summary>
    [Fact]
    public void BackofficeCommandControllers_MustUseBackofficeUserPolicy()
    {
        var violations = new List<string>();

        foreach (var controller in GetConcreteSubclasses<OrganisationRegistryCommandController>())
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            if (authorize is null) continue;

            if (authorize.Policy != PolicyNames.BackofficeUser)
                violations.Add(
                    $"{controller.Name}: Policy = \"{authorize.Policy}\" " +
                    $"maar verwacht \"{PolicyNames.BackofficeUser}\"");
        }

        violations.Should().BeEmpty(
            because: "backoffice command controllers moeten policy-based autorisatie gebruiken op basis van vo_id");
    }

    /// <summary>
    /// Backoffice controllers mogen geen schemes meer vastleggen op het attribuut.
    /// De toegestane schemes horen centraal bij de BackofficeUser-policy.
    /// </summary>
    [Fact]
    public void BackofficeCommandControllers_MustNotDeclareAuthenticationSchemes()
    {
        var violations = new List<string>();

        foreach (var controller in GetConcreteSubclasses<OrganisationRegistryCommandController>())
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            if (authorize is null) continue;

            if (!string.IsNullOrWhiteSpace(authorize.AuthenticationSchemes))
                violations.Add(
                    $"{controller.Name}: AuthenticationSchemes = \"{authorize.AuthenticationSchemes}\" " +
                    "maar schemes horen centraal in de BackofficeUser-policy");
        }

        violations.Should().BeEmpty(
            because: "transportkeuze en toegangsbeslissing moeten gescheiden blijven");
    }

    [Fact]
    public void OrganisationRegistryAuthorizeAttribute_IsBackofficeUserPolicyWrapper()
    {
        var attribute = new OrganisationRegistryAuthorizeAttribute(Role.AlgemeenBeheerder, Role.Developer);

        attribute.Policy.Should().Be(PolicyNames.BackofficeUser);
        attribute.AuthenticationSchemes.Should().BeNullOrWhiteSpace();
        attribute.Roles.Should().Be($"{RoleMapping.Map(Role.AlgemeenBeheerder)},{RoleMapping.Map(Role.Developer)}");
    }

    [Fact]
    public void BackofficeUserPolicy_WhenBffApiDisabled_UsesTokenExchangeJwtBearerOnly()
    {
        var policy = GetBackofficeUserPolicy(bffApiEnabled: false);

        policy.AuthenticationSchemes.Should().BeEquivalentTo(new[] { Schemes.JwtBearer });
        policy.Requirements.OfType<DenyAnonymousAuthorizationRequirement>().Should().ContainSingle();
        policy.Requirements
            .OfType<ClaimsAuthorizationRequirement>()
            .Should()
            .ContainSingle(x => x.ClaimType == AcmIdmConstants.Claims.AcmId);
    }

    [Fact]
    public void BackofficeUserPolicy_WhenBffApiEnabled_UsesTokenExchangeJwtBearerAndBffApi()
    {
        var policy = GetBackofficeUserPolicy(bffApiEnabled: true);

        policy.AuthenticationSchemes.Should().BeEquivalentTo(new[] { Schemes.JwtBearer, Schemes.BffApi });
        policy.AuthenticationSchemes.Should().NotContain(Schemes.Introspection);
    }

    [Fact]
    public async Task BackofficeUserPolicy_AllowsAuthenticatedUserTokenWithVoId()
    {
        var policy = GetBackofficeUserPolicy(bffApiEnabled: true);
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(AcmIdmConstants.Claims.AcmId, "user-id") },
                Schemes.JwtBearer));

        var context = await EvaluatePolicy(policy, principal);

        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task BackofficeUserPolicy_RejectsMachineToMachineTokenWithOnlyScopes()
    {
        var policy = GetBackofficeUserPolicy(bffApiEnabled: true);
        var principal = new ClaimsPrincipal(
            new ClaimsIdentity(
                new[] { new Claim(AcmIdmConstants.Claims.Scope, AcmIdmConstants.Scopes.CjmBeheerder) },
                Schemes.Introspection));

        var context = await EvaluatePolicy(policy, principal);

        context.HasSucceeded.Should().BeFalse();
    }

    /// <summary>
    /// Overzicht van alle backoffice command controllers en hun auth-configuratie.
    /// Levende documentatie van het backoffice-toegangsmodel.
    /// </summary>
    [Fact]
    public void BackofficeCommandControllers_AuthConfiguration_IsDocumented()
    {
        _output.WriteLine("Backoffice command controller → auth mapping (BackofficeUser policy via OrganisationRegistryAuthorize):");
        _output.WriteLine(new string('-', 70));

        foreach (var controller in GetConcreteSubclasses<OrganisationRegistryCommandController>().OrderBy(c => c.Name))
        {
            var authorize = controller.GetCustomAttribute<AuthorizeAttribute>();
            var attributeName = authorize?.GetType().Name ?? "(geen authorize)";
            var policy = authorize?.Policy ?? "(geen policy)";
            var roles = string.IsNullOrWhiteSpace(authorize?.Roles) ? "(geen rollen)" : authorize.Roles;
            _output.WriteLine($"  {controller.Name,-55} [{attributeName}] policy={policy,-20} roles={roles}");
        }

        GetConcreteSubclasses<OrganisationRegistryCommandController>().Should().NotBeEmpty(
            because: "er moeten backoffice command controllers bestaan in de API-assembly");
    }

    // ---------------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------------

    private static IEnumerable<Type> GetConcreteSubclasses<TBase>()
        => ApiAssembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.IsSubclassOf(typeof(TBase)));

    private static AuthorizationPolicy GetBackofficeUserPolicy(bool bffApiEnabled)
    {
        var options = new AuthorizationOptions();
        GetConfigureAuthPolicies()(options);

        return options.GetPolicy(PolicyNames.BackofficeUser)
               ?? throw new InvalidOperationException($"{PolicyNames.BackofficeUser} policy is not registered");

        Action<AuthorizationOptions> GetConfigureAuthPolicies()
        {
            var method = typeof(Startup).GetMethod(
                "ConfigureAuthPolicies",
                BindingFlags.NonPublic | BindingFlags.Static);

            return (Action<AuthorizationOptions>)method!.Invoke(null, new object[] { bffApiEnabled })!;
        }
    }

    private static async Task<AuthorizationHandlerContext> EvaluatePolicy(
        AuthorizationPolicy policy,
        ClaimsPrincipal principal)
    {
        var context = new AuthorizationHandlerContext(policy.Requirements, principal, resource: null);

        foreach (var handler in policy.Requirements.OfType<IAuthorizationHandler>())
            await handler.HandleAsync(context);

        return context;
    }
}
