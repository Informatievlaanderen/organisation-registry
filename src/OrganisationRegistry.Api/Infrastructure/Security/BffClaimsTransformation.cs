namespace OrganisationRegistry.Api.Infrastructure.Security;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Api.Security;
using Microsoft.AspNetCore.Authentication;
using OrganisationRegistry.Infrastructure.Authorization;

/// <summary>
/// Zet claims uit de OAuth2 introspection response (BffApi scheme) om naar
/// ClaimTypes.Role claims die door [OrganisationRegistryAuthorize] gebruikt worden.
///
/// De introspection response bevat iv_wegwijs_rol_3D claims die vertaald moeten worden
/// naar de interne Role claims, analoog aan OrganisationRegistryTokenBuilder.ParseRoles.
/// </summary>
public class BffClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        // Alleen transformeren voor BffApi-geauthenticeerde principals
        if (!principal.Identities.Any(i => i.AuthenticationType == AuthenticationSchemes.BffApi))
            return Task.FromResult(principal);

        // Kloon de eerste ClaimsIdentity (de BffApi identity)
        var bffIdentity = principal.Identities.First(i => i.AuthenticationType == AuthenticationSchemes.BffApi);
        var identity = (ClaimsIdentity)bffIdentity.Clone();
        var cloned = new ClaimsPrincipal(identity);

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
