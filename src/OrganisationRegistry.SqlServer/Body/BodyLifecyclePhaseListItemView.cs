namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Body.Events;
    using LifecyclePhaseType;
    using OrganisationRegistry.LifecyclePhaseType.Events;

    using System.Linq;
    using Microsoft.Extensions.Logging;

    public class BodyLifecyclePhaseListItem
    {
        public Guid BodyLifecyclePhaseId { get; set; }
        public Guid BodyId { get; set; }

        public Guid LifecyclePhaseTypeId { get; set; }
        public string LifecyclePhaseTypeName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool HasAdjacentGaps { get; set; }
    }

    public class BodyLifecyclePhaseListConfiguration : EntityMappingConfiguration<BodyLifecyclePhaseListItem>
    {
        public override void Map(EntityTypeBuilder<BodyLifecyclePhaseListItem> b)
        {
            b.ToTable(nameof(BodyLifecyclePhaseListView.ProjectionTables.BodyLifecyclePhaseList), "OrganisationRegistry")
                .HasKey(p => p.BodyLifecyclePhaseId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.LifecyclePhaseTypeId).IsRequired();
            b.Property(p => p.LifecyclePhaseTypeName).HasMaxLength(LifecyclePhaseTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.HasAdjacentGaps);

            b.HasIndex(x => x.LifecyclePhaseTypeName).ForSqlServerIsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class BodyLifecyclePhaseListView :
        Projection<BodyLifecyclePhaseListView>,
        IEventHandler<BodyLifecyclePhaseAdded>,
        IEventHandler<BodyLifecyclePhaseUpdated>,
        IEventHandler<LifecyclePhaseTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyLifecyclePhaseList
        }

        private readonly IEventStore _eventStore;

        public BodyLifecyclePhaseListView(
            ILogger<BodyLifecyclePhaseListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LifecyclePhaseTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyLifecyclePhases = context.BodyLifecyclePhaseList.Where(x => x.LifecyclePhaseTypeId == message.Body.LifecyclePhaseTypeId);
                if (!bodyLifecyclePhases.Any())
                    return;

                foreach (var bodyLifecyclePhase in bodyLifecyclePhases)
                    bodyLifecyclePhase.LifecyclePhaseTypeName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseAdded> message)
        {
            var bodyLifecyclePhaseListItem = new BodyLifecyclePhaseListItem
            {
                BodyLifecyclePhaseId = message.Body.BodyLifecyclePhaseId,
                BodyId = message.Body.BodyId,
                LifecyclePhaseTypeId = message.Body.LifecyclePhaseTypeId,
                LifecyclePhaseTypeName = message.Body.LifecyclePhaseTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.BodyLifecyclePhaseList.Add(bodyLifecyclePhaseListItem);
                context.SaveChanges();
            }

            UpdateLifecyclePhaseGaps(dbConnection, dbTransaction, message.Body.BodyId);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyLifecyclePhaseUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyLifecyclePhase = context.BodyLifecyclePhaseList.SingleOrDefault(item => item.BodyLifecyclePhaseId == message.Body.BodyLifecyclePhaseId);

                bodyLifecyclePhase.BodyLifecyclePhaseId = message.Body.BodyLifecyclePhaseId;
                bodyLifecyclePhase.BodyId = message.Body.BodyId;
                bodyLifecyclePhase.LifecyclePhaseTypeId = message.Body.LifecyclePhaseTypeId;
                bodyLifecyclePhase.LifecyclePhaseTypeName = message.Body.LifecyclePhaseTypeName;
                bodyLifecyclePhase.ValidFrom = message.Body.ValidFrom;
                bodyLifecyclePhase.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }

            UpdateLifecyclePhaseGaps(dbConnection, dbTransaction, message.Body.BodyId);
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private void UpdateLifecyclePhaseGaps(DbConnection dbConnection, DbTransaction dbTransaction, Guid bodyId)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var sortedLifecyclePhases = context
                    .BodyLifecyclePhaseList
                    .Where(x => x.BodyId == bodyId)
                    .OrderBy(x => x.ValidFrom)
                    .ToList();

                // If there is no lifecycle, we dont care
                if (sortedLifecyclePhases.Count == 0)
                    return;

                // Assume all is fine!
                foreach (var bodyLifecyclePhase in sortedLifecyclePhases)
                    bodyLifecyclePhase.HasAdjacentGaps = false;

                // Should start with a fixed startdate
                var firstLifecyclePhase = sortedLifecyclePhases.First();
                if (!firstLifecyclePhase.ValidFrom.HasValue)
                    firstLifecyclePhase.HasAdjacentGaps = true;

                // Should end with an infinite enddate
                var lastLifecyclePhase = sortedLifecyclePhases.Last();
                if (lastLifecyclePhase.ValidTo.HasValue)
                    lastLifecyclePhase.HasAdjacentGaps = true;

                // All periouds should be continuous
                var lifecyclePhasesWithGaps = sortedLifecyclePhases
                    .Skip(1)
                    .Zip(sortedLifecyclePhases, (current, previous) => new {
                        Current = current,
                        Previous = previous,
                        EndDate = previous.ValidTo,
                        StartDate = current.ValidFrom
                    })
                    .Where(x => x.EndDate.Value.AddDays(1) != x.StartDate)
                    .ToList();

                foreach (var lifecyclePhasesWithGap in lifecyclePhasesWithGaps)
                {
                    lifecyclePhasesWithGap.Current.HasAdjacentGaps = true;
                    lifecyclePhasesWithGap.Previous.HasAdjacentGaps = true;
                }

                context.SaveChanges();
            }
        }
    }
}
