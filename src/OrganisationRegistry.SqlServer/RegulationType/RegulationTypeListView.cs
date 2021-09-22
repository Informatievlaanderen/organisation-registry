namespace OrganisationRegistry.SqlServer.RegulationType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.RegulationType.Events;

    public class RegulationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class RegulationTypeListConfiguration : EntityMappingConfiguration<RegulationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<RegulationTypeListItem> b)
        {
            b.ToTable(nameof(RegulationTypeListView.ProjectionTables.RegulationTypeList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class RegulationTypeListView :
        Projection<RegulationTypeListView>,
        IEventHandler<RegulationTypeCreated>,
        IEventHandler<RegulationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            RegulationTypeList
        }

        private readonly IEventStore _eventStore;

        public RegulationTypeListView(
            ILogger<RegulationTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationTypeCreated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulationType = new RegulationTypeListItem
            {
                Id = message.Body.RegulationTypeId,
                Name = message.Body.Name,
            };

            await context.RegulationTypeList.AddAsync(regulationType);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var regulationType = context.RegulationTypeList.SingleOrDefault(x => x.Id == message.Body.RegulationTypeId);
            if (regulationType == null)
                return; // TODO: Error?

            regulationType.Name = message.Body.Name;
            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
