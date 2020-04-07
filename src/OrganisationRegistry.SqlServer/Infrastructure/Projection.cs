namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;

    public abstract class BaseProjection<T>
    {
        protected readonly ILogger<T> Logger;

        protected BaseProjection(ILogger<T> logger)
        {
            Logger = logger;

            Logger.LogTrace("Created EventHandler {ProjectionName}.", typeof(T));
        }
    }

    public interface IProjectionMarker { }

    public abstract class Projection<T> : BaseProjection<T>, IProjectionMarker, IEventHandler<RebuildProjection>
    {
        protected readonly IContextFactory ContextFactory;
        public abstract string[] ProjectionTableNames { get; }

        protected Projection(ILogger<T> logger, IContextFactory contextFactory) : base(logger)
        {
            ContextFactory = contextFactory;
        }

        private const int BatchSize = 5000;

        public abstract void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message);

        public void RebuildProjection(IEventStore eventStore, DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(eventStore, dbConnection, dbTransaction, message, _ => { });
        }

        public void RebuildProjection(IEventStore eventStore, DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message, Action<OrganisationRegistryContext> customResetLogic)
        {
            if (message.Body.ProjectionName != typeof(T).FullName)
                return;

            var eventTypes = this
                .GetType()
                .GetInterfaces()
                .Where(x => x.GetTypeInfo().IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IEventHandler<>))
                .Select(x => x.GetGenericArguments().First())
                .Except(new[] { typeof(RebuildProjection), typeof(Rollback), typeof(ResetMemoryCache) })
                .ToArray();

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} started.", ProjectionTableNames, message.Body.ProjectionName);

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                customResetLogic(context);

                while (DeleteRows(context) > 0)
                { }
            }

            Logger.LogInformation("Initialization {ProjectionTableNames} for {ProjectionName} finished.", ProjectionTableNames, message.Body.ProjectionName);

            //get last event number
            var lastEvent = eventStore.GetLastEvent();

            //theoretical maximum of iterations
            var iterations = (int)Math.Ceiling((double)lastEvent / (double)BatchSize);

            Logger.LogInformation("Projection rebuild for {ProjectionName} started.", message.Body.ProjectionName);

            var lastProcessed = 0;
            for (var iteration = 0; iteration <= iterations; iteration++)
            {
                var envelopes = eventStore.GetEventEnvelopesAfter(lastProcessed, BatchSize, eventTypes.ToArray()).ToList();
                var envelopeCount = envelopes.Count;

                foreach (var envelope in envelopes)
                {
                    ((dynamic)this).Handle(dbConnection, dbTransaction, (dynamic)envelope);

                    lastProcessed = envelope.Number;
                }

                //if envelopeCount is smaller than BatchSize, the last event was processed
                if (envelopeCount < BatchSize)
                    break;
            }

            Logger.LogInformation("Projection rebuild for {ProjectionName} finished.", message.Body.ProjectionName);
        }

        private int DeleteRows(DbContext context)
        {
            return context.Database.ExecuteSqlCommand(
                string.Concat(ProjectionTableNames.Select(tableName =>
                    $"DELETE TOP(500) FROM [OrganisationRegistry].[{tableName}];")));
        }
    }
}
