namespace OrganisationRegistry.SqlServer.Capacity
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Capacity.Events;

    public class CapacityListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CapacityListConfiguration : EntityMappingConfiguration<CapacityListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<CapacityListItem> b)
        {
            b.ToTable(nameof(CapacityListView.ProjectionTables.CapacityList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
        }
    }

    public class CapacityListView :
        Projection<CapacityListView>,
        IEventHandler<CapacityCreated>,
        IEventHandler<CapacityUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            CapacityList
        }

        private readonly IEventStore _eventStore;

        public CapacityListView(
            ILogger<CapacityListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityCreated> message)
        {
            var capacityType = new CapacityListItem
            {
                Id = message.Body.CapacityId,
                Name = message.Body.Name,
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.CapacityList.Add(capacityType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var capacity = context.CapacityList.SingleOrDefault(x => x.Id == message.Body.CapacityId);
                if (capacity == null)
                    return; // TODO: Error?

                capacity.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
