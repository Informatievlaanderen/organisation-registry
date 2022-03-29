namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SqlServer;
    using SqlServer.ElasticSearchProjections;
    using OrganisationRegistry.SqlServer.Infrastructure;

    public interface IPersonHandlerCache
    {
        Task ClearCache(OrganisationRegistryContext context);
        Task<Dictionary<Guid, string>> GetContactTypeNames(OrganisationRegistryContext context);

        Task<OrganisationCacheItem> GetOrganisation(
            OrganisationRegistryContext context,
            Guid organisationId);

        Task<List<Guid>> GetOrganisationIdsShownOnVlaamseOverheidSite(OrganisationRegistryContext context);
        Task<List<IsActivePerOrganisationCapacity>> IsActivePerOrganisationCapacity(OrganisationRegistryContext context);
        Task AddShowOnVlaamseOverheidSites(
            IContextFactory contextFactory,
            Guid organisationId,
            bool showOnVlaamseOverheidSites);

        Task UpdateShowOnVlaamseOverheidSites(IContextFactory contextFactory, Guid organisationId, bool showOnVlaamseOverheidSites);

        Task<CachedOrganisationBody> GetOrganisationForBodyFromCache(
            OrganisationRegistryContext organisationRegistryContext,
            Guid bodyId);

        Task UpdateIsActivePerOrganisationCapacity(IContextFactory contextFactory, Guid organisationCapacityId, bool isActive);
    }
}
