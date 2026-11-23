namespace OrganisationRegistry.Api.Security;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Duende.IdentityModel;
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

        // Map user name claims from JWT to ACM-IDM format
        var familyName = identity.GetOptionalClaim(JwtClaimTypes.FamilyName);
        if (!string.IsNullOrEmpty(familyName))
            identity.AddClaim(new Claim(AcmIdmConstants.Claims.FamilyName, familyName, ClaimValueTypes.String));

        var givenName = identity.GetOptionalClaim(JwtClaimTypes.GivenName);
        if (!string.IsNullOrEmpty(givenName))
            identity.AddClaim(new Claim(AcmIdmConstants.Claims.Firstname, givenName, ClaimValueTypes.String));

        var developers = (_configuration.Developers ?? string.Empty)
            .Split([';'], StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.ToLowerInvariant());

        var acmIdClaim = identity.GetOptionalClaim(AcmIdmConstants.Claims.AcmId);

        if (!string.IsNullOrEmpty(acmIdClaim) &&
            developers.Contains(acmIdClaim.ToLowerInvariant()))
            AddRoleClaim(identity, Role.Developer);

        // Wraps (does not copy) the identity: the iv_wegwijs_rol_3D claims are parsed on each lookup.
        var user = new ClaimsPrincipal(identity);

        if (user.HasRole(AcmIdmConstants.Roles.AlgemeenBeheerder))
        {
            AddRoleClaim(identity, Role.AlgemeenBeheerder);
            return identity;
        }

        if (user.HasRole(AcmIdmConstants.Roles.VlimpersBeheerder))
            AddRoleClaim(identity, Role.VlimpersBeheerder);

        // If any of the roles is wegwijs body admin, you are one, regardless on which OVO you got it
        if (user.HasRole(AcmIdmConstants.Roles.OrgaanBeheerder))
            AddRoleClaim(identity, Role.OrgaanBeheerder);

        AddRoleWithOrganisationClaims(user, identity, AcmIdmConstants.Roles.DecentraalBeheerder, Role.DecentraalBeheerder);

        if (user.HasRole(AcmIdmConstants.Roles.RegelgevingBeheerder))
            AddRoleClaim(identity, Role.RegelgevingBeheerder);

        if (user.HasAnyRole(AcmIdmConstants.Roles.VoMedewerkerLegacy, AcmIdmConstants.Roles.VoMedewerker))
            AddRoleClaim(identity, Role.VoMedewerker);

        AddRoleWithOrganisationClaims(user, identity, AcmIdmConstants.Roles.VoDecentraalBeheerder, Role.VoDecentraalBeheerder);

        return identity;
    }

    private static void AddRoleWithOrganisationClaims(ClaimsPrincipal user, ClaimsIdentity identity, string acmRole, Role role)
    {
        var ovoCodes = user.GetOvoCodes(acmRole);
        if (ovoCodes.Count == 0)
            return;

        AddRoleClaim(identity, role);

        foreach (var ovoCode in ovoCodes)
            AddOrganisationClaim(identity, ovoCode);
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
        if (!identity.HasClaim(AcmIdmConstants.Claims.Organisation, value))
            identity.AddClaim(claim);
    }
}
