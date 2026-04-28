namespace OrganisationRegistry.UnitTests;

using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;
using Xunit.Abstractions;

public class SecurityServiceRoleMappingTests
{
    private readonly ITestOutputHelper _output;

    public SecurityServiceRoleMappingTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void RoleMapping_ShouldMapAlgemeenBeheerderCorrectly()
    {
        // Arrange
        var roleString = "algemeenbeheerder";

        // Act
        var exists = RoleMapping.Exists(roleString);
        var mappedRole = exists ? RoleMapping.Map(roleString) : (Role?)null;

        // Assert
        _output.WriteLine($"Role string: '{roleString}'");
        _output.WriteLine($"Exists: {exists}");
        _output.WriteLine($"Mapped to: {mappedRole}");

        exists.Should().BeTrue("algemeenbeheerder should exist in RoleMapping");
        mappedRole.Should().Be(Role.AlgemeenBeheerder);
    }

    [Fact]
    public void SimulateSecurityServiceGetSecurityInformation_WithJwtBearerToken()
    {
        // Arrange - Create a ClaimsPrincipal as it would come from JWT Bearer authentication
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "api-integration-tests"),
            new Claim(ClaimTypes.GivenName, "Algemeenbeheerder"),
            new Claim(ClaimTypes.Surname, "Persona"),
            new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
            new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"),
            new Claim(ClaimTypes.Role, "algemeenbeheerder"), // This is added by ParseRoles
        };

        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        _output.WriteLine("=== PRINCIPAL CLAIMS ===");
        foreach (var claim in principal.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        // Act - Simulate what SecurityService.GetSecurityInformation does
        var roleClaims = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        _output.WriteLine("\n=== ROLE CLAIMS ===");
        foreach (var roleClaim in roleClaims)
        {
            _output.WriteLine($"Role claim value: '{roleClaim}'");
        }

        var mappedRoles = roleClaims
            .Where(RoleMapping.Exists)
            .Select(roleString =>
            {
                var exists = RoleMapping.Exists(roleString);
                var mapped = exists ? RoleMapping.Map(roleString) : (Role?)null;
                _output.WriteLine($"  - '{roleString}' -> Exists: {exists}, Mapped: {mapped}");
                return mapped;
            })
            .Where(r => r.HasValue)
            .Select(r => r!.Value)
            .ToImmutableArray();

        _output.WriteLine($"\n=== FINAL MAPPED ROLES ===");
        foreach (var role in mappedRoles)
        {
            _output.WriteLine($"Role: {role}");
        }

        // Assert
        mappedRoles.Should().NotBeEmpty("at least one role should be mapped");
        mappedRoles.Should().Contain(Role.AlgemeenBeheerder, "AlgemeenBeheerder role should be present");
    }

    [Fact]
    public void SimulateAngularUIScenario_AfterTokenExchange()
    {
        // Arrange - Simulate what happens after token exchange
        // The token has been validated and claims have been transformed
        var claims = new[]
        {
            new Claim(JwtClaimTypes.Subject, "api-integration-tests"),
            new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"),
            new Claim(JwtClaimTypes.FamilyName, "Persona"),
            new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
            new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"),
            new Claim(AcmIdmConstants.Claims.Id, "api-integration-tests"),
            new Claim(AcmIdmConstants.Claims.Firstname, "Algemeenbeheerder"),
            new Claim(AcmIdmConstants.Claims.FamilyName, "Persona"),
            new Claim(ClaimTypes.Role, "algemeenbeheerder"), // Added by OrganisationRegistryTokenBuilder.ParseRoles
        };

        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme,
            ClaimTypes.NameIdentifier, ClaimTypes.Role);
        var principal = new ClaimsPrincipal(identity);

        _output.WriteLine("=== SIMULATED ANGULAR UI REQUEST ===");
        _output.WriteLine($"Authentication Type: {identity.AuthenticationType}");
        _output.WriteLine($"Is Authenticated: {identity.IsAuthenticated}");
        _output.WriteLine($"Name Claim Type: {identity.NameClaimType}");
        _output.WriteLine($"Role Claim Type: {identity.RoleClaimType}");

        _output.WriteLine("\n=== ALL CLAIMS ===");
        foreach (var claim in principal.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        // Act - What SecurityService does
        var firstName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var lastName = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;

        _output.WriteLine($"\n=== EXTRACTED INFO ===");
        _output.WriteLine($"FirstName (ClaimTypes.GivenName): {firstName}");
        _output.WriteLine($"LastName (ClaimTypes.Surname): {lastName}");

        var roles = principal.Claims
            .Where(c => c.Type == ClaimTypes.Role)
            .Select(c => c.Value)
            .Where(RoleMapping.Exists)
            .Select(RoleMapping.Map)
            .ToImmutableArray();

        _output.WriteLine($"\n=== FINAL ROLES ===");
        _output.WriteLine($"Count: {roles.Length}");
        foreach (var role in roles)
        {
            _output.WriteLine($"Role: {role}");
        }

        // Assert
        firstName.Should().BeNull("JWT tokens use given_name, not the standard ClaimTypes.GivenName");
        lastName.Should().BeNull("JWT tokens use family_name, not the standard ClaimTypes.Surname");
        roles.Should().NotBeEmpty("roles should be mapped");
        roles.Should().Contain(Role.AlgemeenBeheerder);
    }

    [Fact]
    public void IdentifyTheProblem_NameClaimsMissing()
    {
        // This test identifies the actual problem:
        // JWT validation maps claims differently than expected by SecurityService

        // Arrange - JWT after validation (as shown in debug test)
        var claims = new[]
        {
            // These are the actual claims from the validated JWT
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", "api-integration-tests"),
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "Algemeenbeheerder"),  // Note: lowercase
            new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "Persona"),
            new Claim("vo_id", "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"),
            new Claim("iv_wegwijs_rol_3D", "WegwijsBeheerder-algemeenbeheerder:OVO002949"),
            new Claim("urn:be:vlaanderen:dienstverlening:acmid", "api-integration-tests"),
            new Claim("urn:be:vlaanderen:acm:familienaam", "Persona"),
            new Claim("urn:be:vlaanderen:acm:voornaam", "Algemeenbeheerder"),
            new Claim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "algemeenbeheerder"),
        };

        var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        _output.WriteLine("=== ACTUAL JWT VALIDATED CLAIMS ===");
        foreach (var claim in principal.Claims)
        {
            _output.WriteLine($"Type: '{claim.Type}', Value: '{claim.Value}'");
        }

        // Act - What SecurityService.GetSecurityInformation actually tries to do
        _output.WriteLine($"\n=== CHECKING FOR EXPECTED CLAIMS ===");

        var givenNameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
        _output.WriteLine($"ClaimTypes.GivenName ('{ClaimTypes.GivenName}'): {givenNameClaim?.Value ?? "NOT FOUND"}");

        var surnameClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
        _output.WriteLine($"ClaimTypes.Surname ('{ClaimTypes.Surname}'): {surnameClaim?.Value ?? "NOT FOUND"}");

        var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        _output.WriteLine($"ClaimTypes.Role ('{ClaimTypes.Role}'): {roleClaims.Count} found");
        foreach (var roleClaim in roleClaims)
        {
            _output.WriteLine($"  - '{roleClaim.Value}'");
        }

        // Assert
        givenNameClaim.Should().NotBeNull("GivenName claim should exist after JWT validation");
        surnameClaim.Should().NotBeNull("Surname claim should exist after JWT validation");
        roleClaims.Should().NotBeEmpty("Role claims should exist");

        var mappedRoles = roleClaims
            .Select(c => c.Value)
            .Where(RoleMapping.Exists)
            .Select(RoleMapping.Map)
            .ToImmutableArray();

        mappedRoles.Should().Contain(Role.AlgemeenBeheerder, "AlgemeenBeheerder role should be mapped");
    }
}
