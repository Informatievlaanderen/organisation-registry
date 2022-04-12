namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
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

        public OrganisationRegistryTokenBuilder(OpenIdConnectConfigurationSection configuration)
        {
            _configuration = configuration;
        }

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
            identity.AddClaim(
                new Claim(
                    OrganisationRegistryClaims.ClaimUserId,
                    identity.GetOptionalClaim(JwtClaimTypes.Subject),
                    ClaimValueTypes.String));

            identity.AddClaim(
                new Claim(OrganisationRegistryClaims.ClaimName, JwtClaimTypes.FamilyName, ClaimValueTypes.String));

            identity.AddClaim(
                new Claim(OrganisationRegistryClaims.ClaimFirstname, JwtClaimTypes.GivenName, ClaimValueTypes.String));

            var roles = identity.GetClaims(OrganisationRegistryClaims.ClaimRoles)
                .Select(x => x.ToLowerInvariant())
                .Where(x => x.StartsWith(OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix))
                .Select(x => x.Replace(OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix, ""))
                .ToList();

            var developers = (_configuration.Developers ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.ToLowerInvariant());

            var acmIdClaim = identity.GetOptionalClaim(OrganisationRegistryClaims.ClaimAcmId);

            if (!string.IsNullOrEmpty(acmIdClaim) &&
                developers.Contains(acmIdClaim.ToLowerInvariant()))
                AddRoleClaim(identity, OrganisationRegistryApiClaims.Developer);

            if (!roles.Any())
                return identity;

            if (roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryAlgemeenBeheerderRole)))
            {
                AddOrganisationRegistryBeheerderClaim(identity);
            }
            else
            {
                AddVlimpersBeheerderClaim(roles, identity);
                AddOrgaanBeheerderClaim(roles, identity);
                AddOrganisatieBeheerderClaim(roles, identity);
            }

            return identity;
        }

        private static void AddVlimpersBeheerderClaim(IEnumerable<string> roles, ClaimsIdentity identity)
        {
            if (!roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryVlimpersBeheerderRole)))
                return;

            AddRoleClaim(identity, OrganisationRegistryApiClaims.VlimpersBeheerder);
        }

        private static void AddOrgaanBeheerderClaim(IList<string> roles, ClaimsIdentity identity)
        {
            // If any of the roles is wegwijs body admin, you are one, regardless on which OVO you got it

            if (!roles.Any(x => x.Contains(OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole)))
                return;

            AddRoleClaim(identity, OrganisationRegistryApiClaims.OrgaanBeheerder);

            for (var i = 0; i < roles.Count; i++)
                roles[i] = roles[i].Replace(
                    OrganisationRegistryClaims.OrganisationRegistryOrgaanBeheerderRole,
                    string.Empty);
        }

        private static void AddOrganisatieBeheerderClaim(IReadOnlyCollection<string> roles, ClaimsIdentity identity)
        {
            if (!roles.Any(IsDecentraalBeheerder))
                return;

            // If any of the roles is admin, you are an admin and we add the organisations separatly
            AddRoleClaim(identity, OrganisationRegistryApiClaims.DecentraalBeheerder);

            var adminRoles = roles.Where(IsDecentraalBeheerder);
            foreach (var role in adminRoles)
            {
                // OrganisationRegistryBeheerder-algemeenbeheerder,beheerder:OVO002949
                var organisation = role.Replace(
                    OrganisationRegistryClaims.OrganisationRegistryBeheerderPrefix,
                    string.Empty).Split(':')[1];
                AddOrganisationClaim(identity, organisation);
            }
        }

        private static bool IsDecentraalBeheerder(string role)
            => role.StartsWith(OrganisationRegistryClaims.OrganisationRegistryBeheerderRole)
               || role.StartsWith(OrganisationRegistryClaims.OrganisationRegistryDecentraalBeheerderRole);

        private static void AddOrganisationRegistryBeheerderClaim(ClaimsIdentity identity)
        {
            AddRoleClaim(identity, OrganisationRegistryApiClaims.AlgemeenBeheerder);
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
