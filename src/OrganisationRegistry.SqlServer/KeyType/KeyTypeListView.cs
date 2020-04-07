namespace OrganisationRegistry.SqlServer.KeyType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using KeyTypes.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;

    public class KeyTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class KeyTypeListConfiguration : EntityMappingConfiguration<KeyTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<KeyTypeListItem> b)
        {
            b.ToTable(nameof(KeyTypeListView.ProjectionTables.KeyTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class KeyTypeListView :
        Projection<KeyTypeListView>,
        IEventHandler<KeyTypeCreated>,
        IEventHandler<KeyTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            KeyTypeList
        }

        private readonly IEventStore _eventStore;

        public KeyTypeListView(
            ILogger<KeyTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeCreated> message)
        {
            var keyType = new KeyTypeListItem
            {
                Id = message.Body.KeyTypeId,
                Name = message.Body.Name,
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.KeyTypeList.Add(keyType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var keyType = context.KeyTypeList.SingleOrDefault(x => x.Id == message.Body.KeyTypeId);
                if (keyType == null)
                    return; // TODO: Error?

                keyType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
