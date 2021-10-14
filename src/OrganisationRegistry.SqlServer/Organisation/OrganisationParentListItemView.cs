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
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;

    public class OrganisationParentListItem
    {
        public Guid OrganisationOrganisationParentId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid ParentOrganisationId { get; set; }
        public string ParentOrganisationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationParentListConfiguration : EntityMappingConfiguration<OrganisationParentListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationParentListItem> b)
        {
            b.ToTable(nameof(OrganisationParentListView.ProjectionTables.OrganisationParentList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.OrganisationOrganisationParentId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.ParentOrganisationId).IsRequired();
            b.Property(p => p.ParentOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ParentOrganisationName).IsClustered();
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationParentListView :
        Projection<OrganisationParentListView>,
        IEventHandler<OrganisationParentAdded>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminated>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            OrganisationParentList
        }

        private readonly IEventStore _eventStore;
        public OrganisationParentListView(
            ILogger<OrganisationParentListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            UpdateParentOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        }

        private static void UpdateParentOrganisationName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IContextFactory contextFactory,
            Guid organisationId,
            string organisationName)
        {
            using (var context = contextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisations = context.OrganisationParentList.Where(x => x.ParentOrganisationId == organisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.ParentOrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
        {
            var organisationParentListItem = new OrganisationParentListItem
            {
                OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
                OrganisationId = message.Body.OrganisationId,
                ParentOrganisationId = message.Body.ParentOrganisationId,
                ParentOrganisationName = message.Body.ParentOrganisationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationParentList.AddAsync(organisationParentListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var key = context.OrganisationParentList.SingleOrDefault(item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                key.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
                key.OrganisationId = message.Body.OrganisationId;
                key.ParentOrganisationId = message.Body.ParentOrganisationId;
                key.ParentOrganisationName = message.Body.ParentOrganisationName;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var parents = context.OrganisationParentList.Where(item =>
                    message.Body.FieldsToTerminate.Parents.Keys.Contains(item.OrganisationOrganisationParentId));

                foreach (var parent in parents)
                    parent.ValidTo = message.Body.FieldsToTerminate.Parents[parent.OrganisationOrganisationParentId];

                await context.SaveChangesAsync();
            }
        }
    }
}
