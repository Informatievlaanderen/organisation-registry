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
            b.ToTable(nameof(LocationListView.ProjectionTables.KeyTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
        }
    }

    public class LocationListView :
        Projection<LocationListView>,
        IEventHandler<KeyTypeCreated>,
        IEventHandler<KeyTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            KeyTypeList
        }

        private readonly IEventStore _eventStore;

        public LocationListView(
            ILogger<LocationListView> logger,
            IEventStore eventStore) : base(logger)
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

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.KeyTypeList.Add(keyType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
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
