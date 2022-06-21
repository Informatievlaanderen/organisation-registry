namespace OrganisationRegistry.ElasticSearch.Projections.Cache;

using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organisation.Events;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;

public class OrganisationCache :
    BaseProjection<OrganisationCache>,
    IEventHandler<OrganisationCreated>,
    IEventHandler<OrganisationCreatedFromKbo>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<InitialiseProjection>
{
    private readonly IContextFactory _contextFactory;

    public OrganisationCache(
        ILogger<OrganisationCache> logger,
        IContextFactory contextFactory) : base(logger)
    {
        _contextFactory = contextFactory;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCreated> message)
    {
        var organisation = new OrganisationCacheItem
        {
            Id = message.Body.OrganisationId,
            Name = message.Body.Name,
            OvoNumber = message.Body.OvoNumber,
        };

        await using var context = _contextFactory.Create();
        await context.OrganisationCache.AddAsync(organisation);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCreatedFromKbo> message)
    {
        var organisation = new OrganisationCacheItem
        {
            Id = message.Body.OrganisationId,
            Name = message.Body.Name,
            OvoNumber = message.Body.OvoNumber,
        };

        await using var context = _contextFactory.Create();
        await context.OrganisationCache.AddAsync(organisation);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdated> message)
    {
        await using var context = _contextFactory.Create();
        var organisation = await context
            .OrganisationCache
            .FindRequiredAsync(message.Body.OrganisationId);

        organisation.Name = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationNameUpdated> message)
    {
        await using var context = _contextFactory.Create();
        var organisation = await context
            .OrganisationCache
            .FindRequiredAsync(message.Body.OrganisationId);

        organisation.Name = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        await using var context = _contextFactory.Create();
        var organisation = await context
            .OrganisationCache
            .FindRequiredAsync(message.Body.OrganisationId);

        organisation.Name = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        await using var context = _contextFactory.Create();
        var organisation = await context
            .OrganisationCache
            .FindRequiredAsync(message.Body.OrganisationId);

        organisation.Name = message.Body.NameBeforeKboCoupling;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<InitialiseProjection> message)
    {
        if (message.Body.ProjectionName != CacheRunner.ProjectionName)
            return;

        Logger.LogInformation("Rebuilding index for {ProjectionName}", message.Body.ProjectionName);
        Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started",
            OrganisationCacheForEsConfiguration.TableName, message.Body.ProjectionName);

        await using (var context = _contextFactory.Create())
            while (await DeleteRows(context) > 0)
            {
            }

        Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished",
            OrganisationCacheForEsConfiguration.TableName, message.Body.ProjectionName);
    }

    private static async Task<int> DeleteRows(DbContext context)
    {
        return await context.Database.ExecuteSqlRawAsync(
            string.Concat(new[] {OrganisationCacheForEsConfiguration.TableName}.Select(tableName =>
                $"DELETE TOP(500) FROM [{OrganisationRegistry.Infrastructure.WellknownSchemas.ElasticSearchProjectionsSchema}].[{tableName}];")));
    }

}
