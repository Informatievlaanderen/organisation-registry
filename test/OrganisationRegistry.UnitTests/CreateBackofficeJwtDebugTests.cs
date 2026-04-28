namespace OrganisationRegistry.UnitTests;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using IdentityModel;
using Microsoft.IdentityModel.Tokens;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Xunit;
using Xunit.Abstractions;

public class CreateBackofficeJwtDebugTests
{
    private readonly ITestOutputHelper _output;

    public CreateBackofficeJwtDebugTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Debug_JWT_Creation_And_Validation()
    {
        // Arrange - Use realistic configuration from integration tests (appsettings.json)
        var openIdConnectConfiguration = new OpenIdConnectConfigurationSection
        {
            JwtSharedSigningKey = "keycloak-demo-local-dev-secret-key-32b",
            JwtAudience = "organisatieregister",
            JwtIssuer = "organisatieregister",
            JwtExpiresInMinutes = 120
        };

        // Step 1: Create identity with claims (zoals in ApiFixture)
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, "api-integration-tests"));
        identity.AddClaim(new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"));
        identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, "Persona"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"));

        _output.WriteLine("=== ORIGINAL IDENTITY CLAIMS ===");
        foreach (var claim in identity.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        // Step 2: Parse roles and build JWT
        var tokenBuilder = new OrganisationRegistryTokenBuilder(openIdConnectConfiguration);
        var parsedIdentity = tokenBuilder.ParseRoles(identity);

        _output.WriteLine("\n=== AFTER ParseRoles() ===");
        foreach (var claim in parsedIdentity.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        var jwt = tokenBuilder.BuildJwt(parsedIdentity);
        _output.WriteLine($"\n=== JWT TOKEN ===\n{jwt}");

        // Step 3: Decode JWT to see what's inside
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(jwt);

        _output.WriteLine("\n=== DECODED JWT CLAIMS ===");
        foreach (var claim in decodedToken.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        // Step 4: Validate the JWT (zoals de API dat doet)
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(openIdConnectConfiguration.JwtSharedSigningKey)),
            ValidateIssuer = true,
            ValidIssuer = openIdConnectConfiguration.JwtIssuer,
            ValidateAudience = true,
            ValidAudience = openIdConnectConfiguration.JwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(5),
            RoleClaimType = ClaimTypes.Role  // <-- Dit is belangrijk!
        };

        var principal = handler.ValidateToken(jwt, validationParameters, out var validatedToken);

        _output.WriteLine("\n=== VALIDATED PRINCIPAL CLAIMS ===");
        foreach (var claim in principal.Claims)
        {
            _output.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
        }

        _output.WriteLine($"\n=== ROLE CLAIMS (using ClaimTypes.Role: '{ClaimTypes.Role}') ===");
        var roleClaims = principal.Claims.Where(c => c.Type == ClaimTypes.Role).ToList();
        foreach (var roleClaim in roleClaims)
        {
            _output.WriteLine($"Role: {roleClaim.Value}");
        }

        // Assertions
        _output.WriteLine("\n=== ASSERTIONS ===");

        // Check if role claim exists in decoded JWT
        var roleClaimInJwt = decodedToken.Claims.FirstOrDefault(c => c.Type == "role");
        _output.WriteLine($"Role claim in JWT: {roleClaimInJwt?.Value ?? "NOT FOUND"}");
        roleClaimInJwt.Should().NotBeNull("the role claim should exist in the JWT");
        roleClaimInJwt!.Value.Should().Be("algemeenbeheerder");

        // Check if role claim is accessible via ClaimTypes.Role after validation
        var roleClaimAfterValidation = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        _output.WriteLine($"Role claim after validation (ClaimTypes.Role): {roleClaimAfterValidation?.Value ?? "NOT FOUND"}");

        if (roleClaimAfterValidation == null)
        {
            _output.WriteLine($"\nWARNING: Role claim not found using ClaimTypes.Role!");
            _output.WriteLine($"Looking for claims with 'role' in the type...");
            var roleRelatedClaims = principal.Claims.Where(c => c.Type.Contains("role", StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var claim in roleRelatedClaims)
            {
                _output.WriteLine($"  - Type: '{claim.Type}', Value: '{claim.Value}'");
            }
        }

        roleClaimAfterValidation.Should().NotBeNull("the role claim should be accessible after validation");
        roleClaimAfterValidation!.Value.Should().Be("algemeenbeheerder");
    }
}
