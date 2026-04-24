namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Security;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Zet claims uit de OAuth2 introspection response (BffApi scheme) om naar
/// de interne claims die door SecurityService en [OrganisationRegistryAuthorize] gebruikt worden.
///
/// De introspection response bevat:
/// - given_name / family_name  → ClaimTypes.GivenName / ClaimTypes.Surname
/// - vo_id                     → AcmIdmConstants.Claims.AcmId (blijft onveranderd)
/// - iv_wegwijs_rol_3D         → ClaimTypes.Role (via RoleMapping)
/// </summary>
public class BffClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Zoek een introspection identity: dit is elke identity die given_name bevat maar
        // nog geen ClaimTypes.GivenName heeft. Bearer JWT-tokens gaan via ParseRoles en
        // hebben ClaimTypes.GivenName al; introspection responses hebben JwtClaimTypes.GivenName.
        // We filteren NIET op AuthenticationType omdat de OAuth2Introspection library de naam
        // niet altijd consistent instelt afhankelijk van de versie.
        var introspectionIdentity = principal.Identities
            .FirstOrDefault(i =>
                i.FindFirst(JwtClaimTypes.GivenName) != null &&
                i.FindFirst(ClaimTypes.GivenName) == null);

        if (introspectionIdentity == null)
            return Task.FromResult(principal);

        // Kloon de introspection identity en bouw een nieuwe principal
        var identity = (ClaimsIdentity)introspectionIdentity.Clone();
        var cloned = new ClaimsPrincipal(identity);

        // Map given_name / family_name naar ClaimTypes.GivenName / ClaimTypes.Surname
        // De introspection library doet geen OIDC claim mapping, dus we doen het zelf.
        MapNameClaim(identity, JwtClaimTypes.GivenName, ClaimTypes.GivenName);
        MapNameClaim(identity, JwtClaimTypes.FamilyName, ClaimTypes.Surname);

        var roles = identity.Claims
            .Where(c => c.Type == AcmIdmConstants.Claims.Role)
            .Select(c => c.Value.ToLowerInvariant())
            .Where(v => v.StartsWith(AcmIdmConstants.RolePrefix))
            .Select(v => v.Replace(AcmIdmConstants.RolePrefix, ""))
            .ToList();

        if (!roles.Any())
            return Task.FromResult(cloned);

        if (roles.Any(r => r.Contains(AcmIdmConstants.Roles.AlgemeenBeheerder)))
        {
            AddRoleClaim(identity, Role.AlgemeenBeheerder);
        }
        else
        {
            if (roles.Any(r => r.Contains(AcmIdmConstants.Roles.VlimpersBeheerder)))
                AddRoleClaim(identity, Role.VlimpersBeheerder);

            if (roles.Any(r => r.Contains(AcmIdmConstants.Roles.OrgaanBeheerder)))
                AddRoleClaim(identity, Role.OrgaanBeheerder);

            if (roles.Any(r => r.Contains(AcmIdmConstants.Roles.CjmBeheerder)))
                AddRoleClaim(identity, Role.CjmBeheerder);

            if (roles.Any(r => r.Contains(AcmIdmConstants.Roles.RegelgevingBeheerder)))
                AddRoleClaim(identity, Role.RegelgevingBeheerder);

            // DecentraalBeheerder: voeg ook organisatie-claims toe
            var decentraalRoles = roles.Where(r => r.StartsWith(AcmIdmConstants.Roles.DecentraalBeheerder)).ToList();
            if (decentraalRoles.Any())
            {
                AddRoleClaim(identity, Role.DecentraalBeheerder);

                foreach (var role in decentraalRoles)
                {
                    var parts = role.Split(':');
                    if (parts.Length > 1)
                        AddOrganisationClaim(identity, parts[1]);
                }
            }
        }

        return Task.FromResult(cloned);
    }

    private static void MapNameClaim(ClaimsIdentity identity, string sourceType, string targetType)
    {
        // Sla over als het target-type al aanwezig is
        if (identity.FindFirst(targetType) != null)
            return;

        var source = identity.FindFirst(sourceType);
        if (source != null)
            identity.AddClaim(new Claim(targetType, source.Value, ClaimValueTypes.String));
    }

    private static void AddRoleClaim(ClaimsIdentity identity, Role value)
    {
        var role = RoleMapping.Map(value);
        if (!identity.HasClaim(ClaimTypes.Role, role))
            identity.AddClaim(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
    }

    private static void AddOrganisationClaim(ClaimsIdentity identity, string ovoNumber)
    {
        if (!identity.HasClaim(AcmIdmConstants.Claims.Organisation, ovoNumber))
            identity.AddClaim(new Claim(AcmIdmConstants.Claims.Organisation, ovoNumber, ClaimValueTypes.String));
    }
}
