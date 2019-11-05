namespace OrganisationRegistry.SqlServer.BodyClassificationType
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data.Common;
    using System.Linq;
    using OrganisationRegistry.BodyClassificationType.Events;
    using OrganisationRegistry.Infrastructure.Events;

    public class BodyClassificationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class BodyClassificationTypeListConfiguration : EntityMappingConfiguration<BodyClassificationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<BodyClassificationTypeListItem> b)
        {
            b.ToTable(nameof(BodyClassificationTypeListView.ProjectionTables.BodyClassificationTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
        }
    }

    public class BodyClassificationTypeListView :
        Projection<BodyClassificationTypeListView>,
        IEventHandler<BodyClassificationTypeCreated>,
        IEventHandler<BodyClassificationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyClassificationTypeList
        }

        private readonly IEventStore _eventStore;

        public BodyClassificationTypeListView(
            ILogger<BodyClassificationTypeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyClassificationType = new BodyClassificationTypeListItem
                {
                    Id = message.Body.BodyClassificationTypeId,
                    Name = message.Body.Name,
                };

                context.BodyClassificationTypeList.Add(bodyClassificationType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var bodyClassificationType = context.BodyClassificationTypeList.SingleOrDefault(x => x.Id == message.Body.BodyClassificationTypeId);
                if (bodyClassificationType == null)
                    return; // TODO: Error?

                bodyClassificationType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
