// ReSharper disable ClassNeverInstantiated.Global

namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using KeyType;
    using KeyTypes.Events;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;

    public class OrganisationKeyListItem : IRemovable
    {
        public Guid OrganisationKeyId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid KeyTypeId { get; set; }
        public string KeyTypeName { get; set; } = null!;
        public string KeyValue { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool ScheduledForRemoval { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrganisationKeyListConfiguration : EntityMappingConfiguration<OrganisationKeyListItem>
    {
        public const int KeyValueLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationKeyListItem> b)
        {
            b.ToTable(nameof(OrganisationKeyListView.ProjectionTables.OrganisationKeyList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.OrganisationKeyId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.KeyTypeId).IsRequired();
            b.Property(p => p.KeyTypeName).HasMaxLength(KeyTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.KeyValue).HasMaxLength(KeyValueLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.ScheduledForRemoval);

            b.HasIndex(x => x.KeyTypeName).IsClustered();
            b.HasIndex(x => x.KeyValue);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    // ReSharper disable once ClassNeverInstantiated.Global
    public class OrganisationKeyListView :
        Projection<OrganisationKeyListView>,
        IEventHandler<OrganisationKeyAdded>,
        IEventHandler<OrganisationKeyUpdated>,
        IEventHandler<KeyTypeUpdated>,
        IEventHandler<OrganisationTerminatedV2>,
        IEventHandler<KeyTypeRemoved>,
        IEventHandler<OrganisationKeyRemoved>
    {
        protected override string[] ProjectionTableNames
            => Enum.GetNames(typeof(ProjectionTables));

        public override string Schema
            => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            OrganisationKeyList
        }

        private readonly IEventStore _eventStore;

        public OrganisationKeyListView(
            ILogger<OrganisationKeyListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
            => _eventStore = eventStore;

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationKeys = context.OrganisationKeyList.Where(x => x.KeyTypeId == message.Body.KeyTypeId);
            if (!organisationKeys.Any())
                return;

            foreach (var organisationKey in organisationKeys)
                organisationKey.KeyTypeName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyAdded> message)
        {
            var organisationKeyListItem = new OrganisationKeyListItem
            {
                OrganisationKeyId = message.Body.OrganisationKeyId,
                OrganisationId = message.Body.OrganisationId,
                KeyTypeId = message.Body.KeyTypeId,
                KeyValue = message.Body.Value,
                KeyTypeName = message.Body.KeyTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationKeyList.AddAsync(organisationKeyListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var key = await context.OrganisationKeyList.SingleAsync(item => item.OrganisationKeyId == message.Body.OrganisationKeyId);

            key.OrganisationKeyId = message.Body.OrganisationKeyId;
            key.OrganisationId = message.Body.OrganisationId;
            key.KeyTypeId = message.Body.KeyTypeId;
            key.KeyValue = message.Body.Value;
            key.KeyTypeName = message.Body.KeyTypeName;
            key.ValidFrom = message.Body.ValidFrom;
            key.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
            => await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeRemoved> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationKeysToBeRemoved =
                context.OrganisationKeyList.Where(item => item.KeyTypeId == message.Body.KeyTypeId);

            foreach (var organisationKey in organisationKeysToBeRemoved)
                organisationKey.ScheduledForRemoval = true;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyRemoved> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var key = await context.OrganisationKeyList.SingleAsync(item => item.OrganisationKeyId == message.Body.OrganisationKeyId);

            context.OrganisationKeyList.Remove(key);

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            if (message.Body.FieldsToTerminate.Keys == null)
                return;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var keys = context.OrganisationKeyList.Where(
                item =>
                    message.Body.FieldsToTerminate.Keys.ContainsKey(item.OrganisationKeyId));

            foreach (var key in keys)
                key.ValidTo = message.Body.FieldsToTerminate.Keys[key.OrganisationKeyId];

            await context.SaveChangesAsync();
        }
    }
}
