namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using IdentityModel;
    using IdentityModel.Client;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.Extensions.Options;
    using Microsoft.IdentityModel.Tokens;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("security")]
    public class SecurityController : OrganisationRegistryController
    {
        private readonly OpenIdConnectConfiguration _openIdConnectConfiguration;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            IOptions<OpenIdConnectConfiguration> openIdConnectConfiguration,            
            ICommandSender commandSender,
            ILogger<SecurityController> logger)
            : base(commandSender)
        {
            _openIdConnectConfiguration = openIdConnectConfiguration.Value;            
            _logger = logger;
        }

        [HttpGet]
        [OrganisationRegistryAuthorize]
        public IActionResult Get([FromServices] ISecurityService securityService)
        {
            return Ok(securityService.GetSecurityInformation(User));
        }

        [HttpGet("info")]
        public IActionResult Info()
        {
            return Ok(new OidcClientConfiguration(_openIdConnectConfiguration));
        }

        [HttpGet("exchange")]
        public async Task<IActionResult> ExchangeCode(string code)
        {
            var tokenEndpointAddress = $"{_openIdConnectConfiguration.Authority}{_openIdConnectConfiguration.TokenEndPoint}";
            var tokenClient = new TokenClient(
                tokenEndpointAddress,
                _openIdConnectConfiguration.ClientId,
                _openIdConnectConfiguration.ClientSecret);

            var tokenResponse = await tokenClient.RequestAuthorizationCodeAsync(
                code,
                new Uri(
                    _openIdConnectConfiguration.AuthorizationRedirectUri,
                    UriKind.RelativeOrAbsolute).ToString());

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

    public class OrganisationRegistryClaims
    {
        public const string ClaimRoles = "iv_wegwijs_rol_3D";
        public const string ClaimAcmId = "vo_id";
        public const string ClaimFirstname = "urn:be:vlaanderen:acm:voornaam";
        public const string ClaimName = "urn:be:vlaanderen:acm:familienaam";

        public const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";
        public const string ClaimUserId = "urn:be:vlaanderen:dienstverlening:acmid";

        public const string OrganisationRegistryBeheerderPrefix = "wegwijsbeheerder-";

        public const string OrganisationRegistryBeheerderRole = "algemeenbeheerder";
        public const string OrganisationRegistryInvoerderRole = "beheerder";
        public const string OrganisationRegistryOrgaanBeheerderRole = "orgaanbeheerder";
    }

    public class OrganisationRegistryApiClaims
    {
        public const string OrganisationRegistryBeheerder = "wegwijsBeheerder";
        public const string OrgaanBeheerder = "orgaanBeheerder";
        public const string Invoerder = "organisatieBeheerder";
        public const string Developer = "developer";
        public const string AutomatedTask = "automatedTask";
    }

    public interface IOrganisationRegistryTokenBuilder
    {
        string BuildJwt(ClaimsIdentity identity);

        ClaimsIdentity ParseRoles(ClaimsIdentity identity);
    }

    public class OrganisationRegistryTokenBuilder : IOrganisationRegistryTokenBuilder
    {
        private readonly OpenIdConnectConfiguration _configuration;

        public OrganisationRegistryTokenBuilder(OpenIdConnectConfiguration configuration) =>
            _configuration = configuration;

        public string BuildJwt(ClaimsIdentity identity)
        {
            var plainTextSecurityKey = _configuration.JwtSharedSigningKey;
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(plainTextSecurityKey));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = _configuration.JwtAudience,
                Issuer = _configuration.JwtIssuer,
                Subject = identity,
                SigningCredentials = signingCredentials,
                IssuedAt = DateTime.UtcNow,
                Expires = DateTimeOffset.FromUnixTimeSeconds(long.Parse(identity.GetClaim(JwtClaimTypes.Expiration)))
                    .UtcDateTime,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodedToken;
        }

        public ClaimsIdentity ParseRoles(ClaimsIdentity identity)
        {
            // todo: not sure we need to translate the claims.

            identity.AddClaim(new Claim(OrganisationRegistryClaims.ClaimUserId, OrganisationRegistryClaims.ClaimAcmId, ClaimValueTypes.String));
            identity.AddClaim(new Claim(OrganisationRegistryClaims.ClaimName, JwtClaimTypes.FamilyName, ClaimValueTypes.String));
            identity.AddClaim(new Claim(OrganisationRegistryClaims.ClaimFirstname, JwtClaimTypes.GivenName, ClaimValueTypes.String));

            var roles = identity.GetClaims(OrganisationRegistryClaims.ClaimRoles)
                .Select(x => x.ToLowerInvariant())
                .Where(x => x.StartsWith(OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix))
                .Select(x => x.Replace(OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix, ""))
                .ToList();

            var developers = _configuration.Developers?.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLowerInvariant());
            if (developers != null &&
                developers.Contains(identity.GetClaim(OrganisationRegistryClaims.ClaimAcmId).ToLowerInvariant()))
                AddRoleClaim(identity, OrganisationRegistryApiClaims.Developer);

            if (roles.Count <= 0)
                return identity;

            // OrganisationRegistryBeheerder-algemeenbeheerder,beheerder:OVO002949
            // OrganisationRegistryBeheerder-beheerder:OVO001831
            // OrganisationRegistryBeheerder-beheerder:OVO001834
            // OrganisationRegistryBeheerder-orgaanbeheerder:OVO001834 // mag alle organen beheren, OVO nummer is puur technisch en nutteloos.

            if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryBeheerderRole)))
            {
                // If any of the roles is wegwijs super admin, the rest doesnt matter
                AddRoleClaim(identity, OrganisationRegistryApiClaims.OrganisationRegistryBeheerder);
            }
            else
            {
                // If any of the roles is wegwijs body admin, you are one, regardless on which OVO you got it
                if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole)))
                {
                    AddRoleClaim(identity, OrganisationRegistryApiClaims.OrgaanBeheerder);

                    for (var i = 0; i < roles.Count; i++)
                        roles[i] = roles[i].Replace(OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole, string.Empty);
                }

                if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryInvoerderRole)))
                {
                    // If any of the roles is admin, you are an admin and we add the organisations separatly
                    AddRoleClaim(identity, OrganisationRegistryApiClaims.Invoerder);

                    var adminRoles = roles.Where(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryInvoerderRole));
                    foreach (var role in adminRoles)
                    {
                        // OrganisationRegistryBeheerder-algemeenbeheerder,beheerder:OVO002949
                        var organisation = role.Replace(OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix, string.Empty).Split(':')[1];
                        AddOrganisationClaim(identity, organisation);
                    }
                }
            }

            return identity;
        }

        private static void AddRoleClaim(ClaimsIdentity identity, string value)
        {
            var claim = new Claim(ClaimTypes.Role, value, ClaimValueTypes.String);
            if (!identity.HasClaim(ClaimTypes.Role, value))
                identity.AddClaim(claim);
        }

        private static void AddOrganisationClaim(ClaimsIdentity identity, string value)
        {
            var claim = new Claim(OrganisationRegistryClaims.ClaimOrganisation, value, ClaimValueTypes.String);
            if (!identity.HasClaim(ClaimTypes.Role, value))
                identity.AddClaim(claim);
        }
    }
}
