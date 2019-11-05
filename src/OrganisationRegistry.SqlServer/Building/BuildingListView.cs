namespace OrganisationRegistry.SqlServer.Building
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.Infrastructure.Events;

    public class BuildingListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? VimId { get; set; }
    }

    public class BuildingListConfiguration : EntityMappingConfiguration<BuildingListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<BuildingListItem> b)
        {
            b.ToTable(nameof(BuildingListView.ProjectionTables.BuildingList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.VimId);

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
            b.HasIndex(x => x.VimId);
        }
    }

    public class BuildingListView :
        Projection<BuildingListView>,
        IEventHandler<BuildingCreated>,
        IEventHandler<BuildingUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BuildingList
        }

        private readonly IEventStore _eventStore;

        public BuildingListView(
            ILogger<BuildingListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingCreated> message)
        {
            var building = new BuildingListItem
            {
                Id = message.Body.BuildingId,
                Name = message.Body.Name,
                VimId = message.Body.VimId
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.BuildingList.Add(building);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var building = context.BuildingList.SingleOrDefault(x => x.Id == message.Body.BuildingId);
                if (building == null)
                    return; // TODO: Error?

                building.Name = message.Body.Name;
                building.VimId = message.Body.VimId;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
