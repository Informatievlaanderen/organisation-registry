namespace OrganisationRegistry.Api.Security;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using OrganisationRegistry.Infrastructure.Authorization;

public class TokenExchangeClaimsTransformation : IClaimsTransformation
{
    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity?.AuthenticationType != "OAuth2Introspection")
        {
            return Task.FromResult(principal);
        }

        var claimsToAdd = new List<Claim>();
        
        // Check if this is a service account with a known scope
        var scope = principal.FindFirst(AcmIdmConstants.Claims.Scope)?.Value;
        var isServiceAccount = scope switch
        {
            AcmIdmConstants.Scopes.CjmBeheerder or
            AcmIdmConstants.Scopes.OrafinBeheerder or
            AcmIdmConstants.Scopes.TestClient => true,
            _ => false
        };

        // Only map user claims for non-service accounts
        if (!isServiceAccount)
        {
            // Map OAuth2 standard claims to ASP.NET Core standard claim types
            var givenName = principal.FindFirst("given_name")?.Value;
            if (!string.IsNullOrEmpty(givenName))
            {
                claimsToAdd.Add(new Claim(ClaimTypes.GivenName, givenName));
            }

            var familyName = principal.FindFirst("family_name")?.Value;
            if (!string.IsNullOrEmpty(familyName))
            {
                claimsToAdd.Add(new Claim(ClaimTypes.Surname, familyName));
            }
        }

        // Map iv_wegwijs_rol_3D to standard Role claim for authorization
        var wegwijsRole = principal.FindFirst("iv_wegwijs_rol_3D")?.Value;
        if (!string.IsNullOrEmpty(wegwijsRole))
        {
            claimsToAdd.Add(new Claim(ClaimTypes.Role, wegwijsRole));
        }

        // Preserve vo_id claim for authorization (already correctly mapped by introspection)
        // The built-in OAuth2 introspection should preserve this claim as-is

        if (claimsToAdd.Count > 0)
        {
            var claimsIdentity = (ClaimsIdentity)principal.Identity;
            claimsIdentity.AddClaims(claimsToAdd);
        }

        return Task.FromResult(principal);
    }
}