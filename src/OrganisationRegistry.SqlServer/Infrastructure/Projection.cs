namespace OrganisationRegistry.SqlServer.Infrastructure;

using Microsoft.EntityFrameworkCore;
using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Infrastructure.EventStore;

public abstract class BaseProjection<T>
{
    protected readonly ILogger Logger;

    protected BaseProjection(ILogger logger)
    {
        Logger = logger;

        Logger.LogTrace("Created EventHandler {ProjectionName}", typeof(T));
    }
}

public interface IProjectionMarker { }

public abstract class Projection<T> : BaseProjection<T>, IProjectionMarker, IEventHandler<RebuildProjection>
{
    protected readonly IContextFactory ContextFactory;
    protected abstract string[] ProjectionTableNames { get; }
    public abstract string Schema { get; }

    protected Projection(ILogger logger, IContextFactory contextFactory) : base(logger)
    {
        ContextFactory = contextFactory;
    }

    private const int BatchSize = 5000;

    public abstract Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message);

    public async Task RebuildProjection(IEventStore eventStore, DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(eventStore, dbConnection, dbTransaction, message, _ => { });
    }

    public async Task RebuildProjection(IEventStore eventStore, DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message, Action<OrganisationRegistryContext> customResetLogic)
    {
        if (message.Body.ProjectionName != typeof(T).FullName)
            return;

        try
        {
            var eventTypes = GetType()
                .GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(x => x.GetGenericArguments().First())
                .Except(new[] {typeof(RebuildProjection), typeof(Rollback), typeof(ResetMemoryCache)})
                .ToArray();

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started",
                ProjectionTableNames, message.Body.ProjectionName);

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                customResetLogic(context);

                while (DeleteRows(context) > 0)
                {
                }
            }

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished",
                ProjectionTableNames, message.Body.ProjectionName);

            Logger.LogInformation("Projection rebuild for {ProjectionName} started in batches of {BatchSize} events",
                message.Body.ProjectionName, BatchSize);

            var lastProcessed = 0;
            int envelopeCount;
            do
            {
                var envelopes = eventStore
                    .GetEventEnvelopesAfter(lastProcessed, BatchSize, eventTypes.ToArray())
                    .ToList();

                envelopeCount = envelopes.Count;

                foreach (var envelope in envelopes)
                {
                    await ((dynamic)this).Handle(dbConnection, dbTransaction, (dynamic)envelope);
                    lastProcessed = envelope.Number;
                }

                Logger.LogInformation(
                    "Projection rebuild for {ProjectionName} processed up until #{LastProcessed}",
                    message.Body.ProjectionName, lastProcessed);


            } while (envelopeCount == BatchSize); //if envelopeCount is smaller than BatchSize, the last event was processed


            Logger.LogInformation("Projection rebuild for {ProjectionName} finished", message.Body.ProjectionName);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Projection rebuild for {ProjectionName} failed", message.Body.ProjectionName);
            throw;
        }
    }

    private int DeleteRows(DbContext context)
    {
        return context.Database.ExecuteSqlRaw(
            string.Concat(ProjectionTableNames.Select(tableName =>
                $"DELETE TOP(500) FROM [{Schema}].[{tableName}];")));
    }
}