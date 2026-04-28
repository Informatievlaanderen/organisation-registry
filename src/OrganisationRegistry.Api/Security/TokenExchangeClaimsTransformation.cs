namespace OrganisationRegistry.Api.Security;

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using OrganisationRegistry.Infrastructure.Authorization;

public class TokenExchangeClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var isServiceAccount = principal.FindFirst(AcmIdmConstants.Claims.AcmId) is null;

        if (isServiceAccount)
            return Task.FromResult(principal);

        var introspectionIdentity = principal.Identities
            .FirstOrDefault(i =>
                i.FindFirst(JwtClaimTypes.GivenName) != null &&
                i.FindFirst(ClaimTypes.GivenName) == null);

        if (introspectionIdentity == null)
            return Task.FromResult(principal);

        var identity = (ClaimsIdentity)introspectionIdentity.Clone();
        var cloned = new ClaimsPrincipal(identity);

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
