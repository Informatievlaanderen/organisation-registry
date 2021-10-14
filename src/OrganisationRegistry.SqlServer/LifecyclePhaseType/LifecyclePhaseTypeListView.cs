namespace OrganisationRegistry.SqlServer.LifecyclePhaseType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.LifecyclePhaseType;
    using OrganisationRegistry.LifecyclePhaseType.Events;

    public class LifecyclePhaseTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool RepresentsActivePhase { get; set; }

        public bool IsDefaultPhase { get; set; }
    }

    public class LifecyclePhaseTypeListConfiguration : EntityMappingConfiguration<LifecyclePhaseTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<LifecyclePhaseTypeListItem> b)
        {
            b.ToTable(nameof(LifecyclePhaseTypeListView.ProjectionTables.LifecyclePhaseTypeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(x => x.RepresentsActivePhase);
            b.Property(x => x.IsDefaultPhase);

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
            b.HasIndex(x => new { x.RepresentsActivePhase, x.IsDefaultPhase });
        }
    }

    public class LifecyclePhaseTypeListView :
        Projection<LifecyclePhaseTypeListView>,
        IEventHandler<LifecyclePhaseTypeCreated>,
        IEventHandler<LifecyclePhaseTypeUpdated>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            LifecyclePhaseTypeList
        }

        private readonly IEventStore _eventStore;

        public LifecyclePhaseTypeListView(
            ILogger<LifecyclePhaseTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var lifecyclePhaseType = new LifecyclePhaseTypeListItem
                {
                    Id = message.Body.LifecyclePhaseTypeId,
                    Name = message.Body.Name,
                    RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase,
                    IsDefaultPhase = message.Body.Status == LifecyclePhaseTypeStatus.Default
                };

                await context.LifecyclePhaseTypeList.AddAsync(lifecyclePhaseType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var lifecyclePhaseType = context.LifecyclePhaseTypeList.SingleOrDefault(x => x.Id == message.Body.LifecyclePhaseTypeId);
                if (lifecyclePhaseType == null)
                    return; // TODO: Error?

                lifecyclePhaseType.Name = message.Body.Name;
                lifecyclePhaseType.RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;
                lifecyclePhaseType.IsDefaultPhase = message.Body.Status == LifecyclePhaseTypeStatus.Default;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
