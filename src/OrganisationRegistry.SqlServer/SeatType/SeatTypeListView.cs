namespace OrganisationRegistry.SqlServer.SeatType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.SeatType.Events;

    public class SeatTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }
    }

    public class SeatTypeListConfiguration : EntityMappingConfiguration<SeatTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<SeatTypeListItem> b)
        {
            b.ToTable(nameof(SeatTypeListView.ProjectionTables.SeatTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.Order);

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class SeatTypeListView :
        Projection<SeatTypeListView>,
        IEventHandler<SeatTypeCreated>,
        IEventHandler<SeatTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            SeatTypeList
        }

        private readonly IEventStore _eventStore;

        public SeatTypeListView(
            ILogger<SeatTypeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var seatType = new SeatTypeListItem
                {
                    Id = message.Body.SeatTypeId,
                    Name = message.Body.Name,
                    Order=message.Body.Order
                };

                context.SeatTypeList.Add(seatType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var seatType = context.SeatTypeList.SingleOrDefault(x => x.Id == message.Body.SeatTypeId);
                if (seatType == null)
                    return; // TODO: Error?

                seatType.Name = message.Body.Name;
                seatType.Order = message.Body.Order;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
