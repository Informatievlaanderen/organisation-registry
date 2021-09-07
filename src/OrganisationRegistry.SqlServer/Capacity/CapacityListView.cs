namespace OrganisationRegistry.SqlServer.Capacity
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
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
            b.ToTable(nameof(CapacityListView.ProjectionTables.CapacityList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
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
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityCreated> message)
        {
            var capacityType = new CapacityListItem
            {
                Id = message.Body.CapacityId,
                Name = message.Body.Name,
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.CapacityList.AddAsync(capacityType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var capacity = context.CapacityList.SingleOrDefault(x => x.Id == message.Body.CapacityId);
                if (capacity == null)
                    return; // TODO: Error?

                capacity.Name = message.Body.Name;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
