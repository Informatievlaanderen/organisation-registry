namespace OrganisationRegistry.Api.Security;

using System;
using IdentityModel.AspNetCore.OAuth2Introspection;

public static class IntrospectionCaching
{
    public static void Apply(OAuth2IntrospectionOptions o, string keyPrefix, TimeSpan ttl, bool skipTokensWithDots)
    {
        o.EnableCaching = true;         // off by default in 6.x (unverified)
        o.CacheDuration = ttl;
        o.CacheKeyPrefix = keyPrefix;   // the two schemes share one IDistributedCache
        o.SkipTokensWithDots = skipTokensWithDots;   // true for ACM (opaque tokens); false for Keycloak (JWT tokens)
    }
}
