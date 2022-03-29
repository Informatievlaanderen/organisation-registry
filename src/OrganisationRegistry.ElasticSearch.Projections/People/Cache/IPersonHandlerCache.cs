namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SqlServer;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;

    public interface IPersonHandlerCache
    {
        Task ClearCache(OrganisationRegistryContext context);

        Task<Dictionary<Guid, string>> GetContactTypeNames(OrganisationRegistryContext context);

        Task<OrganisationCacheItem> GetOrganisation(OrganisationRegistryContext context, Guid organisationId);

        Task<List<Guid>> GetIdsForOrganisationsShownOnVlaamseOverheidSite(OrganisationRegistryContext context);

        Task<List<IsActivePerOrganisationCapacity>> GetIsActivePerOrganisationCapacity(
            OrganisationRegistryContext context);

        Task UpdateIsActivePerOrganisationCapacity(
            IContextFactory contextFactory,
            Guid organisationCapacityId,
            bool isActive);

        Task AddOrganisationShowOnVlaamseOverheidSites(
            IContextFactory contextFactory,
            Guid organisationId,
            bool showOnVlaamseOverheidSites);

        Task UpdateOrganisationShowOnVlaamseOverheidSites(
            IContextFactory contextFactory,
            Guid organisationId,
            bool showOnVlaamseOverheidSites);

        Task<CachedOrganisationBody> GetOrganisationForBody(
            OrganisationRegistryContext organisationRegistryContext,
            Guid bodyId);
    }
}
