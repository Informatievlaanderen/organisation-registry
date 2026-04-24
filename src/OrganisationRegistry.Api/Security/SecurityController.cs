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

        var tokenEndpointAddress =
            $"{_openIdConnectConfiguration.Authority}{_openIdConnectConfiguration.TokenEndPoint}";

        _logger.LogDebug("Exchanging authorization code at endpoint: {TokenEndpointAddress}", tokenEndpointAddress);

        var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(
            new AuthorizationCodeTokenRequest
            {
                ClientId = _openIdConnectConfiguration.ClientId,
                ClientSecret = _openIdConnectConfiguration.ClientSecret,
                RedirectUri = redirectUri ?? _openIdConnectConfiguration.AuthorizationRedirectUri,
                Address = tokenEndpointAddress,
                Code = code,
                CodeVerifier = verifier,
            },
            cancellationToken);

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
