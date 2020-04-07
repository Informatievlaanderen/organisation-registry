namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class BodyOrganisationListItem
    {
        public Guid BodyOrganisationId { get; set; }
        public Guid BodyId { get; set; }

        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodyOrganisationListConfiguration : EntityMappingConfiguration<BodyOrganisationListItem>
    {
        public override void Map(EntityTypeBuilder<BodyOrganisationListItem> b)
        {
            b.ToTable(nameof(BodyOrganisationListView.ProjectionTables.BodyOrganisationList), "OrganisationRegistry")
                .HasKey(p => p.BodyOrganisationId)
                .IsClustered(false);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.OrganisationId).IsRequired();
            b.Property(p => p.OrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.OrganisationName).IsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class BodyOrganisationListView :
        Projection<BodyOrganisationListView>,
        IEventHandler<BodyOrganisationAdded>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyOrganisationList
        }

        private readonly IEventStore _eventStore;
        private Func<DbConnection, DbTransaction, OrganisationRegistryContext> _contextFactory;

        public BodyOrganisationListView(
            ILogger<BodyOrganisationListView> logger,
            IEventStore eventStore,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory = null) : base(logger)
        {
            _eventStore = eventStore;
            _contextFactory = contextFactory ?? ((connection, transaction) =>
                new OrganisationRegistryTransactionalContext(connection, transaction));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, _contextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, _contextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        private static void UpdateOrganisationName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory,
            Guid organisationId,
            string organisationName)
        {
            using (var context = contextFactory(dbConnection, dbTransaction))
            {
                var organisations = context.BodyOrganisationList.Where(x => x.OrganisationId == organisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.OrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
        {
            var organisationParentListItem = new BodyOrganisationListItem
            {
                BodyOrganisationId = message.Body.BodyOrganisationId,
                BodyId = message.Body.BodyId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationName = message.Body.OrganisationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.BodyOrganisationList.Add(organisationParentListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var organisation = context.BodyOrganisationList.SingleOrDefault(item => item.BodyOrganisationId == message.Body.BodyOrganisationId);

                organisation.BodyOrganisationId = message.Body.BodyOrganisationId;
                organisation.OrganisationId = message.Body.OrganisationId;
                organisation.BodyId = message.Body.BodyId;
                organisation.OrganisationName = message.Body.OrganisationName;
                organisation.ValidFrom = message.Body.ValidFrom;
                organisation.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
