namespace OrganisationRegistry.SqlServer.SeatType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.SeatType.Events;

    public class SeatTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int? Order { get; set; }

        public bool IsEffective { get; set; }
    }

    public class SeatTypeListConfiguration : EntityMappingConfiguration<SeatTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<SeatTypeListItem> b)
        {
            b.ToTable(nameof(SeatTypeListView.ProjectionTables.SeatTypeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.Order);

            b.Property(p => p.IsEffective)
                .HasDefaultValue(true)
                .ValueGeneratedNever();

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
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var seatType = new SeatTypeListItem
                {
                    Id = message.Body.SeatTypeId,
                    Name = message.Body.Name,
                    Order= message.Body.Order,
                    IsEffective =  message.Body.IsEffective ?? true
                };

                await context.SeatTypeList.AddAsync(seatType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var seatType = context.SeatTypeList.Single(x => x.Id == message.Body.SeatTypeId);

                seatType.Name = message.Body.Name;
                seatType.Order = message.Body.Order;
                seatType.IsEffective = message.Body.IsEffective ?? true;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
