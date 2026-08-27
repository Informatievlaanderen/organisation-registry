namespace OrganisationRegistry.UnitTests;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Configuration;
using Xunit;

public class TokenValidationParametersTests
{
    private const string SharedSigningKey = "keycloak-demo-local-dev-secret-key-32b";
    private const string Audience = "organisatieregister";

    private const string ConfiguredIssuer = "organisatieregister";
    private const string AdditionalIssuer = "https://auth.wegwijs.vlaanderen.be";

    [Theory]
    [InlineData(ConfiguredIssuer)]
    [InlineData(AdditionalIssuer)]
    public void EveryConfiguredIssuer_IsAccepted(string issuer)
    {
        var parameters = CreateParameters($"{ConfiguredIssuer}, {AdditionalIssuer}");

        Validating(issuer, parameters).Should().NotThrow();
    }

    [Fact]
    public void AnIssuerThatIsNotConfigured_IsRejected()
    {
        var parameters = CreateParameters($"{ConfiguredIssuer}, {AdditionalIssuer}");

        Validating("https://auth.somewhere.else", parameters)
            .Should().Throw<SecurityTokenInvalidIssuerException>();
    }

    [Fact]
    public void WithoutAdditionalIssuers_OnlyTheSingleIssuerIsAccepted()
    {
        var parameters = CreateParameters(jwtIssuers: null);

        parameters.ValidIssuers.Should().BeNull();

        Validating(ConfiguredIssuer, parameters).Should().NotThrow();
        Validating(AdditionalIssuer, parameters).Should().Throw<SecurityTokenInvalidIssuerException>();
    }

    private static Action Validating(string issuer, TokenValidationParameters parameters)
        => () => new JwtSecurityTokenHandler().ValidateToken(CreateJwt(issuer), parameters, out _);

    private static OrganisationRegistryTokenValidationParameters CreateParameters(string? jwtIssuers)
        => new(
            new OpenIdConnectConfigurationSection
            {
                JwtSharedSigningKey = SharedSigningKey,
                JwtAudience = Audience,
                JwtIssuer = ConfiguredIssuer,
                JwtIssuers = jwtIssuers,
                JwtExpiresInMinutes = 120,
            });

    private static string CreateJwt(string issuer)
        => new JwtSecurityTokenHandler().WriteToken(
            new JwtSecurityToken(
                issuer,
                Audience,
                claims: null,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(5),
                new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SharedSigningKey)),
                    SecurityAlgorithms.HmacSha256)));
}
