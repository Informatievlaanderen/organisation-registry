namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Text;
    using IdentityModel;
    using Microsoft.IdentityModel.Tokens;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class OrganisationRegistryTokenBuilder : IOrganisationRegistryTokenBuilder
    {
        private readonly OpenIdConnectConfigurationSection _configuration;

        public OrganisationRegistryTokenBuilder(OpenIdConnectConfigurationSection configuration) =>
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
                Expires = DateTimeOffset.Now.AddMinutes(_configuration.JwtExpiresInMinutes).UtcDateTime,
                NotBefore = DateTime.UtcNow
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodedToken;
        }

        public ClaimsIdentity ParseRoles(ClaimsIdentity identity)
        {
            identity.AddClaim(new Claim(OrganisationRegistryClaims.ClaimUserId, identity.GetClaim(JwtClaimTypes.Subject), ClaimValueTypes.String));
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
                if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryVlimpersRole)))
                {
                    AddRoleClaim(identity, OrganisationRegistryApiClaims.VlimpersBeheerder);
                }
                // If any of the roles is wegwijs body admin, you are one, regardless on which OVO you got it
                if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole)))
                {
                    AddRoleClaim(identity, OrganisationRegistryApiClaims.OrgaanBeheerder);

                    for (var i = 0; i < roles.Count; i++)
                        roles[i] = roles[i].Replace(OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole, string.Empty);
                }

                if (roles.Any(x => x.StartsWith(OrganisationRegistryClaims.OrganisationRegistryInvoerderRole)))
                {
                    // If any of the roles is admin, you are an admin and we add the organisations separatly
                    AddRoleClaim(identity, OrganisationRegistryApiClaims.Invoerder);

                    var adminRoles = roles.Where(x => x.StartsWith(OrganisationRegistryClaims.OrganisationRegistryInvoerderRole));
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
