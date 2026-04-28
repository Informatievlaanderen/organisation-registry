namespace OrganisationRegistry.UnitTests;

using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using FluentAssertions;
using IdentityModel;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Xunit;

public class CreateBackofficeJwtTests
{
    [Fact]
    public void CreateBackofficeJwt_ShouldCreateValidJwtWithExpectedClaims()
    {
        // Arrange - Use realistic configuration as in ApiFixture
        var openIdConnectConfiguration = new OpenIdConnectConfigurationSection
        {
            JwtSharedSigningKey = "keycloak-demo-local-dev-secret-key-32b",
            JwtAudience = "organisatieregister",
            JwtIssuer = "organisatieregister",
            JwtExpiresInMinutes = 120
        };

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, "api-integration-tests"));
        identity.AddClaim(new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"));
        identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, "Persona"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"));

        var tokenBuilder = new OrganisationRegistryTokenBuilder(openIdConnectConfiguration);

        // Act
        var jwt = tokenBuilder.BuildJwt(tokenBuilder.ParseRoles(identity));

        // Assert
        jwt.Should().NotBeNullOrEmpty();

        // Decode the JWT to verify its contents
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(jwt);

        // Verify standard JWT claims
        decodedToken.Issuer.Should().Be("organisatieregister");
        decodedToken.Audiences.Should().Contain("organisatieregister");

        // Verify custom claims
        decodedToken.Claims.Should().Contain(c => c.Type == JwtClaimTypes.Subject && c.Value == "api-integration-tests");
        decodedToken.Claims.Should().Contain(c => c.Type == JwtClaimTypes.GivenName && c.Value == "Algemeenbeheerder");
        decodedToken.Claims.Should().Contain(c => c.Type == JwtClaimTypes.FamilyName && c.Value == "Persona");
        decodedToken.Claims.Should().Contain(c => c.Type == AcmIdmConstants.Claims.AcmId && c.Value == "A5C5BFCD-266C-4CC7-9869-4B95E34C090D");
        decodedToken.Claims.Should().Contain(c => c.Type == AcmIdmConstants.Claims.Role && c.Value == "WegwijsBeheerder-algemeenbeheerder:OVO002949");

        // Verify the parsed role claim (AlgemeenBeheerder role should be added)
        // Note: JWT serialization shortens ClaimTypes.Role to "role"
        decodedToken.Claims.Should().Contain(c => c.Type == "role" && c.Value == RoleMapping.Map(Role.AlgemeenBeheerder));

        // Verify ACM-IDM claims were added
        decodedToken.Claims.Should().Contain(c => c.Type == AcmIdmConstants.Claims.Id && c.Value == "api-integration-tests");
        decodedToken.Claims.Should().Contain(c => c.Type == AcmIdmConstants.Claims.Firstname && c.Value == "Algemeenbeheerder");
        decodedToken.Claims.Should().Contain(c => c.Type == AcmIdmConstants.Claims.FamilyName && c.Value == "Persona");
    }

    [Fact]
    public void CreateBackofficeJwt_ShouldHaveValidTokenStructure()
    {
        // Arrange - Use realistic configuration as in ApiFixture
        var openIdConnectConfiguration = new OpenIdConnectConfigurationSection
        {
            JwtSharedSigningKey = "keycloak-demo-local-dev-secret-key-32b",
            JwtAudience = "organisatieregister",
            JwtIssuer = "organisatieregister",
            JwtExpiresInMinutes = 120
        };

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, "api-integration-tests"));
        identity.AddClaim(new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"));
        identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, "Persona"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"));

        var tokenBuilder = new OrganisationRegistryTokenBuilder(openIdConnectConfiguration);

        // Act
        var jwt = tokenBuilder.BuildJwt(tokenBuilder.ParseRoles(identity));

        // Assert - JWT should have 3 parts (header.payload.signature)
        var parts = jwt.Split('.');
        parts.Should().HaveCount(3, "a valid JWT should have header, payload, and signature");

        // Verify the token can be parsed without exceptions
        var handler = new JwtSecurityTokenHandler();
        var act = () => handler.ReadJwtToken(jwt);
        act.Should().NotThrow();
    }

    [Fact]
    public void CreateBackofficeJwt_ShouldIncludeAlgemeenBeheerderRole()
    {
        // Arrange - Use realistic configuration as in ApiFixture
        var openIdConnectConfiguration = new OpenIdConnectConfigurationSection
        {
            JwtSharedSigningKey = "keycloak-demo-local-dev-secret-key-32b",
            JwtAudience = "organisatieregister",
            JwtIssuer = "organisatieregister",
            JwtExpiresInMinutes = 120
        };

        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, "api-integration-tests"));
        identity.AddClaim(new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"));
        identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, "Persona"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"));

        var tokenBuilder = new OrganisationRegistryTokenBuilder(openIdConnectConfiguration);

        // Act
        var parsedIdentity = tokenBuilder.ParseRoles(identity);
        var jwt = tokenBuilder.BuildJwt(parsedIdentity);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(jwt);

        // Note: JWT serialization shortens ClaimTypes.Role to "role"
        var roleClaims = decodedToken.Claims
            .Where(c => c.Type == "role")
            .Select(c => c.Value)
            .ToList();

        roleClaims.Should().Contain(RoleMapping.Map(Role.AlgemeenBeheerder),
            "the token should include the AlgemeenBeheerder role based on the input role claim");
    }
}