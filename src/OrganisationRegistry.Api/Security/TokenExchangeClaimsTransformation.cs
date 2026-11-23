namespace OrganisationRegistry.Api.Security;

using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Duende.IdentityModel;
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

        // `cloned` wraps `identity` (not a copy) and holds only that identity,
        // so the iv_wegwijs_rol_3D claims are read from exactly the same claims as before.
        if (cloned.HasRole(AcmIdmConstants.Roles.AlgemeenBeheerder))
        {
            AddRoleClaim(identity, Role.AlgemeenBeheerder);
            return Task.FromResult(cloned);
        }

        if (cloned.HasRole(AcmIdmConstants.Roles.VlimpersBeheerder))
            AddRoleClaim(identity, Role.VlimpersBeheerder);

        if (cloned.HasRole(AcmIdmConstants.Roles.OrgaanBeheerder))
            AddRoleClaim(identity, Role.OrgaanBeheerder);

        if (cloned.HasRole(AcmIdmConstants.Roles.RegelgevingBeheerder))
            AddRoleClaim(identity, Role.RegelgevingBeheerder);

        if (cloned.HasAnyRole(AcmIdmConstants.Roles.VoMedewerkerLegacy, AcmIdmConstants.Roles.VoMedewerker))
            AddRoleClaim(identity, Role.VoMedewerker);

        AddRoleWithOrganisationClaims(cloned, identity, AcmIdmConstants.Roles.VoDecentraalBeheerder, Role.VoDecentraalBeheerder);
        AddRoleWithOrganisationClaims(cloned, identity, AcmIdmConstants.Roles.DecentraalBeheerder, Role.DecentraalBeheerder);

        return Task.FromResult(cloned);
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
        AddClaimIfMissing(identity, identity.RoleClaimType, role);
        AddClaimIfMissing(identity, ClaimTypes.Role, role);
    }

    private static void AddClaimIfMissing(ClaimsIdentity identity, string claimType, string value)
    {
        if (!identity.HasClaim(claimType, value))
            identity.AddClaim(new Claim(claimType, value, ClaimValueTypes.String));
    }

    private static void AddOrganisationClaim(ClaimsIdentity identity, string ovoNumber)
    {
        if (!identity.HasClaim(AcmIdmConstants.Claims.Organisation, ovoNumber))
            identity.AddClaim(new Claim(AcmIdmConstants.Claims.Organisation, ovoNumber, ClaimValueTypes.String));
    }
}
