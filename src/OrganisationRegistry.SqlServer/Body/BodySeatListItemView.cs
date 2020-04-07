namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Body.Events;
    using SeatType;

    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class BodySeatListItem
    {
        public Guid BodySeatId { get; set; }
        public Guid BodyId { get; set; }

        public string Name { get; set; }

        public string BodySeatNumber { get; set; }

        public bool PaidSeat { get; set; }

        public bool EntitledToVote { get; set; }

        public Guid SeatTypeId { get; set; }
        public string SeatTypeName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodySeatListConfiguration : EntityMappingConfiguration<BodySeatListItem>
    {
        public const int NameLength = 500;
        public const int SeatNumberLength = 10;

        public override void Map(EntityTypeBuilder<BodySeatListItem> b)
        {
            b.ToTable(nameof(BodySeatListView.ProjectionTables.BodySeatList), "OrganisationRegistry")
                .HasKey(p => p.BodySeatId)
                .IsClustered(false);

            b.Property(p => p.BodySeatNumber)
                .HasMaxLength(SeatNumberLength);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.Name).HasMaxLength(NameLength).IsRequired();

            b.Property(p => p.PaidSeat);

            b.Property(p => p.EntitledToVote);

            b.Property(p => p.SeatTypeId).IsRequired();
            b.Property(p => p.SeatTypeName)
                .HasMaxLength(SeatTypeListConfiguration.NameLength)
                .IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class BodySeatListView :
        Projection<BodySeatListView>,
        IEventHandler<BodySeatAdded>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<BodySeatNumberAssigned>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodySeatList
        }

        private readonly IEventStore _eventStore;

        public BodySeatListView(
            ILogger<BodySeatListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatAdded> message)
        {
            var bodySeatListItem = new BodySeatListItem
            {
                BodySeatId = message.Body.BodySeatId,
                BodyId = message.Body.BodyId,
                Name = message.Body.Name,
                BodySeatNumber = message.Body.BodySeatNumber,
                SeatTypeId = message.Body.SeatTypeId,
                SeatTypeName = message.Body.SeatTypeName,
                PaidSeat = message.Body.PaidSeat,
                EntitledToVote = message.Body.EntitledToVote,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.BodySeatList.Add(bodySeatListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodySeat = context.BodySeatList.SingleOrDefault(item => item.BodySeatId == message.Body.BodySeatId);

                bodySeat.BodySeatId = message.Body.BodySeatId;
                bodySeat.BodyId = message.Body.BodyId;
                bodySeat.Name = message.Body.Name;
                bodySeat.SeatTypeId = message.Body.SeatTypeId;
                bodySeat.SeatTypeName = message.Body.SeatTypeName;
                bodySeat.PaidSeat = message.Body.PaidSeat;
                bodySeat.EntitledToVote = message.Body.EntitledToVote;
                bodySeat.ValidFrom = message.Body.ValidFrom;
                bodySeat.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatNumberAssigned> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodySeat = context.BodySeatList.SingleOrDefault(item => item.BodySeatId == message.Body.BodySeatId);

                bodySeat.BodySeatNumber = message.Body.BodySeatNumber;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
