namespace OrganisationRegistry.SqlServer.Purpose
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Purpose.Events;

    public class PurposeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class PurposeListConfiguration : EntityMappingConfiguration<PurposeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<PurposeListItem> b)
        {
            b.ToTable(nameof(PurposeListView.ProjectionTables.PurposeList), "OrganisationRegistry")
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
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            PurposeList
        }

        private readonly IEventStore _eventStore;

        public PurposeListView(
            ILogger<PurposeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeCreated> message)
        {
            var purposeType = new PurposeListItem
            {
                Id = message.Body.PurposeId,
                Name = message.Body.Name,
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.PurposeList.Add(purposeType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var purpose = context.PurposeList.SingleOrDefault(x => x.Id == message.Body.PurposeId);
                if (purpose == null)
                    return; // TODO: Error?

                purpose.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
