namespace OrganisationRegistry.Api.Security;

using System;
using System.Collections.Generic;
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
            NotBefore = DateTime.UtcNow,
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
                AcmIdmConstants.Claims.Id,
                identity.GetOptionalClaim(JwtClaimTypes.Subject) ?? string.Empty,
                ClaimValueTypes.String));

        identity.AddClaim(
            new Claim(AcmIdmConstants.Claims.FamilyName, JwtClaimTypes.FamilyName, ClaimValueTypes.String));

        identity.AddClaim(
            new Claim(AcmIdmConstants.Claims.Firstname, JwtClaimTypes.GivenName, ClaimValueTypes.String));

        var roles = identity.GetClaims(AcmIdmConstants.Claims.Role)
            .Select(x => x.ToLowerInvariant())
            .Where(x => x.StartsWith(AcmIdmConstants.RolePrefix))
            .Select(x => x.Replace(AcmIdmConstants.RolePrefix, ""))
            .ToList();

        var developers = (_configuration.Developers ?? string.Empty)
            .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLowerInvariant());

        var acmIdClaim = identity.GetOptionalClaim(AcmIdmConstants.Claims.AcmId);

        if (!string.IsNullOrEmpty(acmIdClaim) &&
            developers.Contains(acmIdClaim.ToLowerInvariant()))
            AddRoleClaim(identity, Role.Developer);

        if (!roles.Any())
            return identity;

        if (roles.Any(x => x.Contains(AcmIdmConstants.Roles.AlgemeenBeheerder)))
        {
            AddOrganisationRegistryBeheerderClaim(identity);
        }
        else
        {
            AddVlimpersBeheerderClaim(roles, identity);
            AddOrgaanBeheerderClaim(roles, identity);
            AddDecentraalBeheerderClaim(roles, identity);
            AddRegelgevingBeheerderClaim(roles, identity);
            AddCjmBeheerderClaim(roles, identity);
        }

        return identity;
    }

    private static void AddCjmBeheerderClaim(IEnumerable<string> roles, ClaimsIdentity identity)
    {
        if (!roles.Any(x => x.Contains(AcmIdmConstants.Roles.CjmBeheerder)))
            return;

        AddRoleClaim(identity, Role.CjmBeheerder);
    }

    private static void AddRegelgevingBeheerderClaim(IEnumerable<string> roles, ClaimsIdentity identity)
    {
        if (!roles.Any(x => x.Contains(AcmIdmConstants.Roles.RegelgevingBeheerder)))
            return;

        AddRoleClaim(identity, Role.RegelgevingBeheerder);
    }

    private static void AddVlimpersBeheerderClaim(IEnumerable<string> roles, ClaimsIdentity identity)
    {
        if (!roles.Any(x => x.Contains(AcmIdmConstants.Roles.VlimpersBeheerder)))
            return;

        AddRoleClaim(identity, Role.VlimpersBeheerder);
    }

    private static void AddOrgaanBeheerderClaim(IList<string> roles, ClaimsIdentity identity)
    {
        // If any of the roles is wegwijs body admin, you are one, regardless on which OVO you got it

        if (!roles.Any(x => x.Contains(AcmIdmConstants.Roles.OrgaanBeheerder)))
            return;

        AddRoleClaim(identity, Role.OrgaanBeheerder);

        for (var i = 0; i < roles.Count; i++)
            roles[i] = roles[i].Replace(
                AcmIdmConstants.Roles.OrgaanBeheerder,
                string.Empty);
    }

    private static void AddDecentraalBeheerderClaim(IReadOnlyCollection<string> roles, ClaimsIdentity identity)
    {
        if (!roles.Any(IsDecentraalBeheerder))
            return;

        AddRoleClaim(identity, Role.DecentraalBeheerder);

        var decentraalBeheerderRoles = roles.Where(IsDecentraalBeheerder);
        foreach (var role in decentraalBeheerderRoles)
        {
            var organisation = role.Replace(
                AcmIdmConstants.RolePrefix,
                string.Empty).Split(':')[1];
            AddOrganisationClaim(identity, organisation);
        }
    }

    private static bool IsDecentraalBeheerder(string role)
        => role.StartsWith(AcmIdmConstants.Roles.DecentraalBeheerder);

    private static void AddOrganisationRegistryBeheerderClaim(ClaimsIdentity identity)
    {
        AddRoleClaim(identity, Role.AlgemeenBeheerder);
    }

    private static void AddRoleClaim(ClaimsIdentity identity, Role value)
    {
        var role = RoleMapping.Map(value);

        var claim = new Claim(ClaimTypes.Role, role, ClaimValueTypes.String);
        if (!identity.HasClaim(ClaimTypes.Role, role))
            identity.AddClaim(claim);
    }

    private static void AddOrganisationClaim(ClaimsIdentity identity, string value)
    {
        var claim = new Claim(AcmIdmConstants.Claims.Organisation, value, ClaimValueTypes.String);
        if (!identity.HasClaim(ClaimTypes.Role, value))
            identity.AddClaim(claim);
    }
}
