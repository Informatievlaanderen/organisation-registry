namespace OrganisationRegistry.Api.Security;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("security")]
public class SecurityController : OrganisationRegistryController
{
    private readonly ILogger<SecurityController> _logger;
    private readonly OpenIdConnectConfigurationSection _openIdConnectConfiguration;

    public SecurityController(
        IOptions<OpenIdConnectConfigurationSection> openIdConnectConfiguration,
        ILogger<SecurityController> logger)
    {
        _openIdConnectConfiguration = openIdConnectConfiguration.Value;
        _logger = logger;
    }
    
    /// <summary>
    /// Gets the effective authority for server-to-server OAuth2 calls.
    /// Uses InternalAuthorityOverride when available for container networking,
    /// otherwise falls back to the regular Authority for external calls.
    /// </summary>
    private string GetEffectiveAuthority()
    {
        var authority = !string.IsNullOrWhiteSpace(_openIdConnectConfiguration.InternalAuthorityOverride)
            ? _openIdConnectConfiguration.InternalAuthorityOverride
            : _openIdConnectConfiguration.Authority;
            
        _logger.LogDebug("Using effective authority for OAuth2 calls: {Authority} (Internal override: {InternalOverride})", 
            authority, _openIdConnectConfiguration.InternalAuthorityOverride ?? "not configured");
            
        return authority;
    }
    
    /// <summary>
    /// Determines if the OAuth2 client is configured as a confidential client (has ClientSecret).
    /// Public clients (SPAs) don't have secrets, confidential clients (server apps) do.
    /// </summary>
    private bool IsConfidentialClient() =>
        !string.IsNullOrWhiteSpace(_openIdConnectConfiguration.ClientSecret);

    /// <summary>
    /// Gets the OAuth2 client type for logging and debugging purposes.
    /// </summary>
    private string GetClientType() =>
        IsConfidentialClient() ? "confidential" : "public";

    [HttpGet]
    [OrganisationRegistryAuthorize]
    public async Task<IActionResult> Get([FromServices] ISecurityService securityService)
        => Ok(await securityService.GetSecurityInformation(User));

    [HttpGet("info")]
    public async Task<IActionResult> Info()
        => await OkAsync(new OidcClientConfiguration(_openIdConnectConfiguration));

    [HttpGet("exchange")]
    public async Task<IActionResult> ExchangeCode(
        [FromServices] IHttpClientFactory httpClientFactory,
        [FromServices] ISecurityService securityService,
        string code,
        string verifier,
        string? redirectUri,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            _logger.LogWarning("Authorization code exchange failed: missing or empty authorization code");
            return BadRequest("Authorization code is required");
        }

        if (string.IsNullOrWhiteSpace(verifier))
        {
            _logger.LogWarning("Authorization code exchange failed: missing or empty PKCE code verifier");
            return BadRequest("PKCE code verifier is required");
        }

        using var httpClient = httpClientFactory.CreateClient();

        var tokenEndpointAddress = $"{GetEffectiveAuthority()}{_openIdConnectConfiguration.TokenEndPoint}";

        _logger.LogDebug("Exchanging authorization code at endpoint: {TokenEndpointAddress} using {ClientType} client", 
            tokenEndpointAddress, GetClientType());

        var tokenRequest = new AuthorizationCodeTokenRequest
        {
            ClientId = _openIdConnectConfiguration.ClientId,
            RedirectUri = redirectUri ?? _openIdConnectConfiguration.AuthorizationRedirectUri,
            Address = tokenEndpointAddress,
            Code = code,
            CodeVerifier = verifier,
        };

        // Only include ClientSecret for confidential clients (public clients use PKCE only)
        if (IsConfidentialClient())
        {
            tokenRequest.ClientSecret = _openIdConnectConfiguration.ClientSecret;
            _logger.LogDebug("Using confidential client authentication with ClientSecret");
        }
        else
        {
            _logger.LogDebug("Using public client authentication with PKCE only (no ClientSecret)");
        }

        var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(tokenRequest, cancellationToken);

        if (tokenResponse.IsError)
        {
            _logger.LogError("OAuth2 authorization code exchange failed. Error: {Error}, ErrorDescription: {ErrorDescription}, TokenEndpoint: {TokenEndpointAddress}", 
                tokenResponse.Error, tokenResponse.ErrorDescription, tokenEndpointAddress);
            
            return tokenResponse.Error switch
            {
                "invalid_grant" => BadRequest($"Invalid authorization code: {tokenResponse.ErrorDescription}"),
                "invalid_client" => BadRequest($"Invalid client configuration: {tokenResponse.ErrorDescription}"),
                "invalid_request" => BadRequest($"Invalid request: {tokenResponse.ErrorDescription}"),
                _ => Problem($"OAuth2 token exchange failed: {tokenResponse.ErrorDescription}")
            };
        }

        var token = new JwtSecurityToken(tokenResponse.IdentityToken);
        var identity = new ClaimsIdentity();
        identity.AddClaims(token.Claims);

        var wegwijsTokenBuilder = new OrganisationRegistryTokenBuilder(_openIdConnectConfiguration);
        identity = wegwijsTokenBuilder.ParseRoles(identity);
        var jwtToken = wegwijsTokenBuilder.BuildJwt(identity);

        var maybeAcmIdClaim = identity.GetOptionalClaim(AcmIdmConstants.Claims.AcmId);
        if (maybeAcmIdClaim is { } acmIdClaim)
            securityService.ExpireUserCache(acmIdClaim);

        return Ok(jwtToken);
    }
}
