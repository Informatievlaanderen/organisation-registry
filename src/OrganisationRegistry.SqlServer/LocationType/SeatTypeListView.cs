namespace OrganisationRegistry.SqlServer.LocationType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.LocationType.Events;

    public class LocationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class LocationTypeListConfiguration : EntityMappingConfiguration<LocationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<LocationTypeListItem> b)
        {
            b.ToTable(nameof(LocationTypeListView.ProjectionTables.LocationTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class LocationTypeListView :
        Projection<LocationTypeListView>,
        IEventHandler<LocationTypeCreated>,
        IEventHandler<LocationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            LocationTypeList
        }

        private readonly IEventStore _eventStore;

        public LocationTypeListView(
            ILogger<LocationTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var locationType = new LocationTypeListItem
                {
                    Id = message.Body.LocationTypeId,
                    Name = message.Body.Name,
                };

                context.LocationTypeList.Add(locationType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var locationType = context.LocationTypeList.SingleOrDefault(x => x.Id == message.Body.LocationTypeId);
                if (locationType == null)
                    return; // TODO: Error?

                locationType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
