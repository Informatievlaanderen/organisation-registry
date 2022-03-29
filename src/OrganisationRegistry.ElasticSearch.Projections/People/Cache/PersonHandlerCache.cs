namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Handlers;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure;
    using SqlServer;
    using SqlServer.ElasticSearchProjections;
    using OrganisationRegistry.SqlServer.Infrastructure;

    public class PersonHandlerCache : IPersonHandlerCache
    {
        private static IEnumerable<string> ProjectionTableNames
            => new[]
            {
                ShowOnVlaamseOverheidSitesPerOrganisationListConfiguration.TableName,
                OrganisationPerBodyListConfiguration.TableName
            };

        public virtual async Task ClearCache(OrganisationRegistryContext context)
            => await context.Database.ExecuteSqlRawAsync(
                string.Concat(
                    ProjectionTableNames.Select(
                        tableName
                            => $"TRUNCATE [{WellknownSchemas.ElasticSearchProjectionsSchema}].[{tableName}];")));

        public async Task<Dictionary<Guid, string>> GetContactTypeNames(OrganisationRegistryContext context)
            => await context.ContactTypeCache
                .Select(x => new { x.Id, x.Name })
                .ToDictionaryAsync(x => x.Id, x => x.Name);

        public async Task<OrganisationCacheItem> GetOrganisation(
            OrganisationRegistryContext context,
            Guid organisationId)
            => await context.OrganisationCache.FindAsync(organisationId);

        public async Task<List<Guid>> GetIdsForOrganisationsShownOnVlaamseOverheidSite(OrganisationRegistryContext context)
            => await context.ShowOnVlaamseOverheidSitesPerOrganisationList
                .AsNoTracking()
                .Where(organisation => organisation.ShowOnVlaamseOverheidSites)
                .Select(organisation => organisation.Id)
                .ToListAsync();

        public async Task<List<IsActivePerOrganisationCapacity>> GetIsActivePerOrganisationCapacity(
            OrganisationRegistryContext context)
            => await context.IsActivePerOrganisationCapacityList
                .AsNoTracking()
                .Where(capacity => capacity.IsActive)
                .ToListAsync();

        public async Task<CachedOrganisationBody> GetOrganisationForBody(
            OrganisationRegistryContext organisationRegistryContext,
            Guid bodyId)
        {
            await using var context = organisationRegistryContext;
            var organisationPerBody =
                await context
                    .OrganisationPerBodyListForES
                    .FindAsync(bodyId);

            return organisationPerBody != null
                ? CachedOrganisationBody.FromCache(organisationPerBody)
                : CachedOrganisationBody.Empty();
        }

        public async Task UpdateIsActivePerOrganisationCapacity(IContextFactory contextFactory, Guid organisationCapacityId, bool isActive)
        {
            await using var context = contextFactory.Create();
            var isActivePerOrganisationCapacity =
                await context
                    .IsActivePerOrganisationCapacityList
                    .FindAsync(organisationCapacityId);

            if (isActivePerOrganisationCapacity == null)
            {
                context
                    .IsActivePerOrganisationCapacityList
                    .Add(new IsActivePerOrganisationCapacity
                    {
                        OrganisationCapacityId = organisationCapacityId,
                        IsActive = isActive
                    });
            }
            else
            {
                isActivePerOrganisationCapacity.IsActive = isActive;
            }

            await context.SaveChangesAsync();
        }


        public async Task AddOrganisationShowOnVlaamseOverheidSites(
            IContextFactory contextFactory,
            Guid organisationId,
            bool showOnVlaamseOverheidSites)
        {
            await using var context = contextFactory.Create();
            if (context.ShowOnVlaamseOverheidSitesPerOrganisationList.Any(x => x.Id == organisationId))
                return;

            context
                .ShowOnVlaamseOverheidSitesPerOrganisationList
                .Add(
                    new ShowOnVlaamseOverheidSitesPerOrganisation
                    {
                        Id = organisationId,
                        ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites
                    });

            await context.SaveChangesAsync();
        }

        public async Task UpdateOrganisationShowOnVlaamseOverheidSites(
            IContextFactory contextFactory,
            Guid organisationId,
            bool showOnVlaamseOverheidSites)
        {
            await using var context = contextFactory.Create();
            var showOnVlaamseOverheidSitesPerOrganisation =
                await context
                    .ShowOnVlaamseOverheidSitesPerOrganisationList
                    .FindAsync(organisationId);

            showOnVlaamseOverheidSitesPerOrganisation.ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites;

            await context.SaveChangesAsync();
        }
    }
}
