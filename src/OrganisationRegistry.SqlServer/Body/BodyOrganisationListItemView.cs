namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class BodyOrganisationListItem
    {
        public Guid BodyOrganisationId { get; set; }
        public Guid BodyId { get; set; }

        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; } = null!;

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodyOrganisationListConfiguration : EntityMappingConfiguration<BodyOrganisationListItem>
    {
        public override void Map(EntityTypeBuilder<BodyOrganisationListItem> b)
        {
            b.ToTable(nameof(BodyOrganisationListView.ProjectionTables.BodyOrganisationList), WellknownSchemas.BackofficeSchema)
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
        IEventHandler<OrganisationNameUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            BodyOrganisationList
        }

        private readonly IEventStore _eventStore;
        public BodyOrganisationListView(
            ILogger<BodyOrganisationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
            await Task.CompletedTask;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
            await Task.CompletedTask;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
            await Task.CompletedTask;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            UpdateOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
            await Task.CompletedTask;
        }

        private static void UpdateOrganisationName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IContextFactory contextFactory,
            Guid organisationId,
            string organisationName)
        {
            using (var context = contextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisations = context.BodyOrganisationList.Where(x => x.OrganisationId == organisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                    organisation.OrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
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

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.BodyOrganisationList.AddAsync(organisationParentListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisation = await context.BodyOrganisationList.SingleAsync(item => item.BodyOrganisationId == message.Body.BodyOrganisationId);

                organisation.BodyOrganisationId = message.Body.BodyOrganisationId;
                organisation.OrganisationId = message.Body.OrganisationId;
                organisation.BodyId = message.Body.BodyId;
                organisation.OrganisationName = message.Body.OrganisationName;
                organisation.ValidFrom = message.Body.ValidFrom;
                organisation.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
