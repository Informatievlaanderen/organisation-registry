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

    public class OrganisationKeyListItem
    {
        public Guid OrganisationKeyId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid KeyTypeId { get; set; }
        public string KeyTypeName { get; set; }
        public string KeyValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

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

            b.HasIndex(x => x.KeyTypeName).IsClustered();
            b.HasIndex(x => x.KeyValue);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationKeyListView :
        Projection<OrganisationKeyListView>,
        IEventHandler<OrganisationKeyAdded>,
        IEventHandler<OrganisationKeyUpdated>,
        IEventHandler<KeyTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationKeyList
        }

        private readonly IEventStore _eventStore;

        public OrganisationKeyListView(
            ILogger<OrganisationKeyListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationKeys = context.OrganisationKeyList.Where(x => x.KeyTypeId == message.Body.KeyTypeId);
                if (!organisationKeys.Any())
                    return;

                foreach (var organisationKey in organisationKeys)
                    organisationKey.KeyTypeName = message.Body.Name;

                await context.SaveChangesAsync();
            }
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

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationKeyList.AddAsync(organisationKeyListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var key = context.OrganisationKeyList.SingleOrDefault(item => item.OrganisationKeyId == message.Body.OrganisationKeyId);

                key.OrganisationKeyId = message.Body.OrganisationKeyId;
                key.OrganisationId = message.Body.OrganisationId;
                key.KeyTypeId = message.Body.KeyTypeId;
                key.KeyValue = message.Body.Value;
                key.KeyTypeName = message.Body.KeyTypeName;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
