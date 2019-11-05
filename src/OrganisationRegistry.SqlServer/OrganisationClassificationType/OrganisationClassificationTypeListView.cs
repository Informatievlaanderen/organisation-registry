namespace OrganisationRegistry.SqlServer.OrganisationClassificationType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.OrganisationClassificationType.Events;

    public class OrganisationClassificationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class OrganisationClassificationTypeListConfiguration : EntityMappingConfiguration<OrganisationClassificationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationClassificationTypeListItem> b)
        {
            b.ToTable(nameof(OrganisationClassificationTypeListView.ProjectionTables.OrganisationClassificationTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
        }
    }

    public class OrganisationClassificationTypeListView :
        Projection<OrganisationClassificationTypeListView>,
        IEventHandler<OrganisationClassificationTypeCreated>,
        IEventHandler<OrganisationClassificationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationClassificationTypeList
        }

        private readonly IEventStore _eventStore;

        public OrganisationClassificationTypeListView(
            ILogger<OrganisationClassificationTypeListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationClassificationType = new OrganisationClassificationTypeListItem
                {
                    Id = message.Body.OrganisationClassificationTypeId,
                    Name = message.Body.Name,
                };

                context.OrganisationClassificationTypeList.Add(organisationClassificationType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationClassificationType = context.OrganisationClassificationTypeList.SingleOrDefault(x => x.Id == message.Body.OrganisationClassificationTypeId);
                if (organisationClassificationType == null)
                    return; // TODO: Error?

                organisationClassificationType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
