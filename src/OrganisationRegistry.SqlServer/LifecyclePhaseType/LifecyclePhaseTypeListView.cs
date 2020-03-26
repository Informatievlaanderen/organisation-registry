namespace OrganisationRegistry.SqlServer.LifecyclePhaseType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
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
            b.ToTable(nameof(LifecyclePhaseTypeListView.ProjectionTables.LifecyclePhaseTypeList), "OrganisationRegistry")
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
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            LifecyclePhaseTypeList
        }

        private readonly IEventStore _eventStore;

        public LifecyclePhaseTypeListView(
            ILogger<LifecyclePhaseTypeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var lifecyclePhaseType = new LifecyclePhaseTypeListItem
                {
                    Id = message.Body.LifecyclePhaseTypeId,
                    Name = message.Body.Name,
                    RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase,
                    IsDefaultPhase = message.Body.Status == LifecyclePhaseTypeStatus.Default
                };

                context.LifecyclePhaseTypeList.Add(lifecyclePhaseType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var lifecyclePhaseType = context.LifecyclePhaseTypeList.SingleOrDefault(x => x.Id == message.Body.LifecyclePhaseTypeId);
                if (lifecyclePhaseType == null)
                    return; // TODO: Error?

                lifecyclePhaseType.Name = message.Body.Name;
                lifecyclePhaseType.RepresentsActivePhase = message.Body.LifecyclePhaseTypeIsRepresentativeFor == LifecyclePhaseTypeIsRepresentativeFor.ActivePhase;
                lifecyclePhaseType.IsDefaultPhase = message.Body.Status == LifecyclePhaseTypeStatus.Default;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
