namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("security")]
    public class SecurityController : OrganisationRegistryController
    {
        private readonly OpenIdConnectConfigurationSection _openIdConnectConfiguration;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            IOptions<OpenIdConnectConfigurationSection> openIdConnectConfiguration,
            ICommandSender commandSender,
            ILogger<SecurityController> logger)
            : base(commandSender)
        {
            _openIdConnectConfiguration = openIdConnectConfiguration.Value;
            _logger = logger;
        }

        [HttpGet]
        [OrganisationRegistryAuthorize]
        public async Task<IActionResult> Get([FromServices] ISecurityService securityService)
        {
            return Ok(await securityService.GetSecurityInformation(User));
        }

        [HttpGet("info")]
        public async Task<IActionResult> Info()
        {
            return Ok(new OidcClientConfiguration(_openIdConnectConfiguration));
        }

        [HttpGet("exchange")]
        public async Task<IActionResult> ExchangeCode(
            [FromServices] IHttpClientFactory httpClientFactory,
            string code,
            string verifier,
            CancellationToken cancellationToken)
        {
            using var httpClient = httpClientFactory.CreateClient();

            var tokenEndpointAddress = $"{_openIdConnectConfiguration.Authority}{_openIdConnectConfiguration.TokenEndPoint}";

            var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
            {
                ClientId = _openIdConnectConfiguration.ClientId,
                ClientSecret = _openIdConnectConfiguration.ClientSecret,
                RedirectUri = _openIdConnectConfiguration.AuthorizationRedirectUri,
                Address = tokenEndpointAddress,
                Code = code,
                CodeVerifier = verifier
            }, cancellationToken);

            if (tokenResponse.IsError)
            {
                var message = $"[Error] {tokenResponse.Error}\n" +
                            $"[ErrorDescription] {tokenResponse.ErrorDescription}\n" +
                            $"[TokenEndpoint] {tokenEndpointAddress}";
                _logger.LogError(message);
                throw new Exception(
                    message,
                    tokenResponse.Exception);
            }

            var token = new JwtSecurityToken(tokenResponse.IdentityToken);
            var identity = new ClaimsIdentity();
            identity.AddClaims(token.Claims);

            var wegwijsTokenBuilder = new OrganisationRegistryTokenBuilder(_openIdConnectConfiguration);
            identity = wegwijsTokenBuilder.ParseRoles(identity);
            var jwtToken = wegwijsTokenBuilder.BuildJwt(identity);

            return Ok(jwtToken);
        }
    }
}
