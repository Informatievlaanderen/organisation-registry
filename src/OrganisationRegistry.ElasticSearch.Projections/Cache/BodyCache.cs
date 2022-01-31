namespace OrganisationRegistry.ElasticSearch.Projections.Cache
{
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;
    using SqlServer.ElasticSearchProjections;

    public class BodyCache :
        BaseProjection<BodyCache>,
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<InitialiseProjection>
    {
        private readonly IContextFactory _contextFactory;

        public BodyCache(
            ILogger<BodyCache> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyRegistered> message)
        {
            var organisation = new BodyCacheItem
            {
                Id = message.Body.BodyId,
                Name = message.Body.Name,
            };

            await using var context = _contextFactory.Create();
            await context.BodyCache.AddAsync(organisation);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodyInfoChanged> message)
        {
            await using var context = _contextFactory.Create();
            var organisation = await context
                .BodyCache
                .FindAsync(message.Body.BodyId);

            organisation.Name = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != CacheRunner.ProjectionName)
                return;

            Logger.LogInformation("Rebuilding index for {ProjectionName}", message.Body.ProjectionName);
            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started",
                BodyCacheForEsConfiguration.TableName, message.Body.ProjectionName);

            await using (var context = _contextFactory.Create())
                while (await DeleteRows(context) > 0)
                {
                }

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished",
                BodyCacheForEsConfiguration.TableName, message.Body.ProjectionName);
        }

        private static async Task<int> DeleteRows(DbContext context)
        {
            return await context.Database.ExecuteSqlRawAsync(
                string.Concat(new[] {BodyCacheForEsConfiguration.TableName}.Select(tableName =>
                    $"DELETE TOP(500) FROM [{OrganisationRegistry.Infrastructure.WellknownSchemas.ElasticSearchProjectionsSchema}].[{tableName}];")));
        }

    }
}
