namespace OrganisationRegistry.SqlServer.Purpose
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Purpose.Events;

    public class PurposeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class PurposeListConfiguration : EntityMappingConfiguration<PurposeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<PurposeListItem> b)
        {
            b.ToTable(nameof(PurposeListView.ProjectionTables.PurposeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class PurposeListView :
        Projection<PurposeListView>,
        IEventHandler<PurposeCreated>,
        IEventHandler<PurposeUpdated>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            PurposeList
        }

        private readonly IEventStore _eventStore;

        public PurposeListView(
            ILogger<PurposeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeCreated> message)
        {
            var purposeType = new PurposeListItem
            {
                Id = message.Body.PurposeId,
                Name = message.Body.Name,
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.PurposeList.AddAsync(purposeType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var purpose = context.PurposeList.SingleOrDefault(x => x.Id == message.Body.PurposeId);
                if (purpose == null)
                    return; // TODO: Error?

                purpose.Name = message.Body.Name;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
