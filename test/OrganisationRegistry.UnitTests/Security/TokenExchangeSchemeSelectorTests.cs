namespace OrganisationRegistry.UnitTests.Security;

using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Api.Security;
using Xunit;

/// <summary>
/// Regression tests for the crash observed on PRD: when the "TokenExchange" configuration section is
/// absent (not yet rolled out), the "BackofficeUser" authorization policy still forced authentication
/// against the "TokenExchange" scheme. That scheme's OAuth2IntrospectionOptions were left with empty
/// Authority/IntrospectionEndpoint values, causing OAuth2IntrospectionOptions.Validate() to throw an
/// unhandled InvalidOperationException on the first request to an endpoint protected by that policy
/// (e.g. GET /v1/security).
/// </summary>
public class TokenExchangeSchemeSelectorTests
{
    [Fact]
    public void IsEnabled_WhenConfigurationIsNull_ReturnsFalse()
    {
        TokenExchangeSchemeSelector.IsEnabled(null).Should().BeFalse();
    }

    [Fact]
    public void IsEnabled_WhenEnabledIsFalse_ReturnsFalse()
    {
        var config = new TokenExchangeConfiguration { Enabled = false };

        TokenExchangeSchemeSelector.IsEnabled(config).Should().BeFalse();
    }

    [Fact]
    public void IsEnabled_WhenEnabledIsTrue_ReturnsTrue()
    {
        var config = new TokenExchangeConfiguration { Enabled = true };

        TokenExchangeSchemeSelector.IsEnabled(config).Should().BeTrue();
    }

    [Fact]
    public void WithTokenExchangeIfEnabled_WhenDisabled_ReturnsBaseSchemesUnchanged()
    {
        var baseSchemes = new[] { JwtBearerDefaults.AuthenticationScheme };

        var result = TokenExchangeSchemeSelector.WithTokenExchangeIfEnabled(baseSchemes, tokenExchangeEnabled: false);

        result.Should().BeEquivalentTo(new[] { JwtBearerDefaults.AuthenticationScheme });
        result.Should().NotContain(AuthenticationSchemes.TokenExchange,
            "the 'TokenExchange' scheme must never be attempted when it is not configured, " +
            "otherwise OAuth2IntrospectionOptions.Validate() throws an unhandled exception at request time");
    }

    [Fact]
    public void WithTokenExchangeIfEnabled_WhenEnabled_AppendsTokenExchange()
    {
        var baseSchemes = new[] { JwtBearerDefaults.AuthenticationScheme };

        var result = TokenExchangeSchemeSelector.WithTokenExchangeIfEnabled(baseSchemes, tokenExchangeEnabled: true);

        result.Should().BeEquivalentTo(new[]
        {
            JwtBearerDefaults.AuthenticationScheme,
            AuthenticationSchemes.TokenExchange,
        });
    }

    [Fact]
    public void WithTokenExchangeIfEnabled_WithMultipleBaseSchemes_AppendsTokenExchangeAtEnd()
    {
        var baseSchemes = new[] { JwtBearerDefaults.AuthenticationScheme, AuthenticationSchemes.EditApi };

        var result = TokenExchangeSchemeSelector.WithTokenExchangeIfEnabled(baseSchemes, tokenExchangeEnabled: true);

        result.Should().BeEquivalentTo(new[]
        {
            JwtBearerDefaults.AuthenticationScheme,
            AuthenticationSchemes.EditApi,
            AuthenticationSchemes.TokenExchange,
        });
    }

    [Fact]
    public void WithTokenExchangeIfEnabled_DoesNotMutateBaseSchemes()
    {
        var baseSchemes = new[] { JwtBearerDefaults.AuthenticationScheme };

        TokenExchangeSchemeSelector.WithTokenExchangeIfEnabled(baseSchemes, tokenExchangeEnabled: true);

        baseSchemes.Should().HaveCount(1, "the original array must not be mutated");
    }
}
