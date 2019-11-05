namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    using System.Linq;
    using FormalFramework;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.FormalFramework.Events;

    public class OrganisationFormalFrameworkListItem
    {
        public Guid OrganisationFormalFrameworkId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid FormalFrameworkId { get; set; }
        public string FormalFrameworkName { get; set; }

        public Guid ParentOrganisationId { get; set; }
        public string ParentOrganisationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationFormalFrameworkListConfiguration : EntityMappingConfiguration<OrganisationFormalFrameworkListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationFormalFrameworkListItem> b)
        {
            b.ToTable(nameof(OrganisationFormalFrameworkListView.ProjectionTables.OrganisationFormalFrameworkList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationFormalFrameworkId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.FormalFrameworkId).IsRequired();
            b.Property(p => p.FormalFrameworkName).HasMaxLength(FormalFrameworkListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ParentOrganisationId).IsRequired();
            b.Property(p => p.ParentOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ParentOrganisationName).ForSqlServerIsClustered();
            b.HasIndex(x => x.FormalFrameworkName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationFormalFrameworkListView :
        Projection<OrganisationFormalFrameworkListView>,
        IEventHandler<OrganisationFormalFrameworkAdded>,
        IEventHandler<OrganisationFormalFrameworkUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<FormalFrameworkUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationFormalFrameworkList
        }

        private readonly IEventStore _eventStore;

        public OrganisationFormalFrameworkListView(
            ILogger<OrganisationFormalFrameworkListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, message.Body.OrganisationId, message.Body.Name);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, message.Body.OrganisationId, message.Body.Name);
        }

        private static void UpdateParentOrganisationName(DbConnection dbConnection, DbTransaction dbTransaction, Guid organisationId, string organisationName)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationFormalFrameworkList
                    .Where(x => x.ParentOrganisationId == organisationId)
                    .ToList()
                    .ForEach(item => item.ParentOrganisationName = organisationName);

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationFormalFrameworks = context.OrganisationFormalFrameworkList.Where(x => x.FormalFrameworkId == message.Body.FormalFrameworkId);
                if (!organisationFormalFrameworks.Any())
                    return;

                foreach (var organisationFormalFramework in organisationFormalFrameworks)
                    organisationFormalFramework.FormalFrameworkName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            var organisationFormalFrameworkListItem = new OrganisationFormalFrameworkListItem
            {
                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                OrganisationId = message.Body.OrganisationId,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                FormalFrameworkName = message.Body.FormalFrameworkName,
                ParentOrganisationId = message.Body.ParentOrganisationId,
                ParentOrganisationName = message.Body.ParentOrganisationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationFormalFrameworkList.Add(organisationFormalFrameworkListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationFormalFramework = context.OrganisationFormalFrameworkList.SingleOrDefault(item => item.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                organisationFormalFramework.OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId;
                organisationFormalFramework.OrganisationId = message.Body.OrganisationId;
                organisationFormalFramework.FormalFrameworkId = message.Body.FormalFrameworkId;
                organisationFormalFramework.FormalFrameworkName = message.Body.FormalFrameworkName;
                organisationFormalFramework.ParentOrganisationId = message.Body.ParentOrganisationId;
                organisationFormalFramework.ParentOrganisationName = message.Body.ParentOrganisationName;
                organisationFormalFramework.ValidFrom = message.Body.ValidFrom;
                organisationFormalFramework.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
