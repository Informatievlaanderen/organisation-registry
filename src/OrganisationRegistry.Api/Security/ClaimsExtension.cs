namespace OrganisationRegistry.Api.Security
{
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Security.Claims;

    public static class ClaimsExtension
    {
        public static void AddOrUpdateClaim(this ClaimsIdentity identity, string key, Claim claim)
        {
            var existingClaim = identity.FindFirst(key);
            if (existingClaim != null)
                identity.RemoveClaim(existingClaim);

            identity.AddClaim(claim);
        }

        public static string? GetOptionalClaim(this ClaimsPrincipal user, string claimType)
            => user.Claims.SingleOrDefault(x => x.Type == claimType)?.Value;

        public static string? GetOptionalClaim(this ClaimsIdentity identity, string claimType)
            => identity.Claims.SingleOrDefault(x => x.Type == claimType)?.Value;

        public static string GetRequiredClaim(this ClaimsPrincipal user, string claimType)
            => user.Claims.Single(x => x.Type == claimType).Value;

        public static string GetRequiredClaim(this ClaimsIdentity identity, string claimType)
            => identity.Claims.Single(x => x.Type == claimType).Value;

        public static IEnumerable<string> GetClaims(this ClaimsPrincipal user, string claimType)
            => user.Claims.Where(x => x.Type == claimType).Select(x => x.Value).ToImmutableList();

        public static IEnumerable<string> GetClaims(this ClaimsIdentity identity, string claimType)
            => identity.Claims.Where(x => x.Type == claimType).Select(x => x.Value).ToImmutableList();
    }
}
