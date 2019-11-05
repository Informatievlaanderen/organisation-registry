namespace OrganisationRegistry.SqlServer.LabelType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.LabelType.Events;

    public class LabelTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class LabelTypeListConfiguration : EntityMappingConfiguration<LabelTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<LabelTypeListItem> b)
        {
            b.ToTable(nameof(LabelTypeListView.ProjectionTables.LabelTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
        }
    }

    public class LabelTypeListView :
        Projection<LabelTypeListView>,
        IEventHandler<LabelTypeCreated>,
        IEventHandler<LabelTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            LabelTypeList
        }

        private readonly IEventStore _eventStore;

        public LabelTypeListView(
            ILogger<LabelTypeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var labelType = new LabelTypeListItem
                {
                    Id = message.Body.LabelTypeId,
                    Name = message.Body.Name,
                };

                context.LabelTypeList.Add(labelType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var labelType = context.LabelTypeList.SingleOrDefault(x => x.Id == message.Body.LabelTypeId);
                if (labelType == null)
                    return; // TODO: Error?

                labelType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}

