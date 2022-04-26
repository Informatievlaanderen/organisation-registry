namespace OrganisationRegistry.Infrastructure.Authorization.Cache
{
    using System;
    using System.Runtime.Caching;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class OrganisationSecurityCache: ICache<OrganisationSecurityInformation>
    {
        private readonly IOrganisationRegistryConfiguration _configuration;
        private MemoryCache _cache;

        public OrganisationSecurityCache(IOrganisationRegistryConfiguration configuration)
        {
            _configuration = configuration;
            _cache = MemoryCache.Default;
        }

        public async Task<OrganisationSecurityInformation> GetOrAdd(string acmId, Func<Task<OrganisationSecurityInformation>> getOrganisationSecurityInformation)
        {
            var maybeCachedSecurityInfo = _cache.Get(acmId);
            if (maybeCachedSecurityInfo is OrganisationSecurityInformation cachedSecurityInfo)
            {
                return cachedSecurityInfo;
            }

            var organisationSecurityInformation = await getOrganisationSecurityInformation();
            Set(acmId, organisationSecurityInformation);
            return organisationSecurityInformation;
        }

        public void Set(string acmId, OrganisationSecurityInformation organisationSecurity)
        {
            _cache.Set(
                new CacheItem(acmId, organisationSecurity),
                new CacheItemPolicy
                {
                    SlidingExpiration =
                        TimeSpan.FromMinutes(_configuration.Caching.UserCacheSlidingExpirationInMinutes),
                });
        }

        public void Expire(string acmId)
        {
            _cache.Remove(acmId);
        }

        public void ExpireAll()
        {
            _cache.Dispose();
            _cache = MemoryCache.Default;
        }
    }
}
