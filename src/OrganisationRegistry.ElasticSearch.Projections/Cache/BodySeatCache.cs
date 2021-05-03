namespace OrganisationRegistry.ElasticSearch.Projections.Cache
{
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;
    using SqlServer.ElasticSearchProjections;

    public class BodySeatCache :
        BaseProjection<BodySeatCache>,
        IEventHandler<BodySeatAdded>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<InitialiseProjection>
    {
        private readonly IContextFactory _contextFactory;

        public BodySeatCache(
            ILogger<BodySeatCache> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatAdded> message)
        {
            var organisation = new BodySeatCacheItem
            {
                Id = message.Body.BodySeatId,
                Name = message.Body.Name,
                Number = message.Body.BodySeatNumber,
                IsPaid = message.Body.PaidSeat
            };

            await using var context = _contextFactory.Create();
            await context.BodySeatCache.AddAsync(organisation);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<BodySeatUpdated> message)
        {
            await using var context = _contextFactory.Create();
            var organisation = await context
                .BodySeatCache
                .SingleAsync(x => x.Id == message.Body.BodySeatId);

            organisation.Name = message.Body.Name;
            organisation.IsPaid = message.Body.PaidSeat;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != CacheRunner.ProjectionName)
                return;

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started.",
                BodySeatCacheForEsConfiguration.TableName, message.Body.ProjectionName);

            await using (var context = _contextFactory.Create())
                while (await DeleteRows(context) > 0)
                {
                }

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished.",
                BodySeatCacheForEsConfiguration.TableName, message.Body.ProjectionName);
        }

        private static async Task<int> DeleteRows(DbContext context)
        {
            return await context.Database.ExecuteSqlRawAsync(
                string.Concat(new[] {BodySeatCacheForEsConfiguration.TableName}.Select(tableName =>
                    $"DELETE TOP(500) FROM [ElasticSearchProjections].[{tableName}];")));
        }

    }
}
