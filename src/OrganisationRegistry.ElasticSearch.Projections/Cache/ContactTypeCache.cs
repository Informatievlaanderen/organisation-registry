namespace OrganisationRegistry.ElasticSearch.Projections.Cache;

using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ContactType.Events;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;
using SqlServer.ElasticSearchProjections;

public class ContactTypeCache :
    BaseProjection<ContactTypeCache>,
    IEventHandler<ContactTypeCreated>,
    IEventHandler<ContactTypeUpdated>,
    IEventHandler<InitialiseProjection>
{
    private readonly IContextFactory _contextFactory;

    public ContactTypeCache(
        ILogger<ContactTypeCache> logger,
        IContextFactory contextFactory) : base(logger)
    {
        _contextFactory = contextFactory;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeCreated> message)
    {
        var contactType = new ContactTypeCacheItem
        {
            Id = message.Body.ContactTypeId,
            Name = message.Body.Name,
        };

        await using var context = _contextFactory.Create();
        await context.ContactTypeCache.AddAsync(contactType);
        await context.SaveChangesAsync();        }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
    {
        await using var context = _contextFactory.Create();
        var contactType = await context
            .ContactTypeCache
            .FindRequiredAsync(message.Body.ContactTypeId);

        contactType.Name = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction,
        IEnvelope<InitialiseProjection> message)
    {
        if (message.Body.ProjectionName != CacheRunner.ProjectionName)
            return;

        Logger.LogInformation("Rebuilding index for {ProjectionName}", message.Body.ProjectionName);
        Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started",
            ContactTypeCacheForEsConfiguration.TableName, message.Body.ProjectionName);

        await using (var context = _contextFactory.Create())
            while (await DeleteRows(context) > 0)
            {
            }

        Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished",
            ContactTypeCacheForEsConfiguration.TableName, message.Body.ProjectionName);
    }

    private static async Task<int> DeleteRows(DbContext context)
    {
        return await context.Database.ExecuteSqlRawAsync(
            string.Concat(new[] {ContactTypeCacheForEsConfiguration.TableName}.Select(tableName =>
                $"DELETE TOP(500) FROM {OrganisationRegistry.Infrastructure.WellknownSchemas.ElasticSearchProjectionsSchema}.[{tableName}];")));
    }

}
