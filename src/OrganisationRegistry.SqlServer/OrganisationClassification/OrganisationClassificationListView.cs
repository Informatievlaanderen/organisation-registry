namespace OrganisationRegistry.SqlServer.OrganisationClassification
{
    using System;
    using System.Linq;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationClassificationType;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.OrganisationClassification.Events;
    using OrganisationRegistry.OrganisationClassificationType.Events;

    public class OrganisationClassificationListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public string? ExternalKey { get; set; }

        public bool Active { get; set; }

        public Guid OrganisationClassificationTypeId { get; set; }
        public string OrganisationClassificationTypeName { get; set; }
    }

    public class OrganisationClassificationListConfiguration : EntityMappingConfiguration<OrganisationClassificationListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationClassificationListItem> b)
        {
            b.ToTable(nameof(OrganisationClassificationListView.ProjectionTables.OrganisationClassificationList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.Order).IsRequired();

            b.Property(p => p.ExternalKey);

            b.Property(p => p.Active);

            b.Property(p => p.OrganisationClassificationTypeId).IsRequired();
            b.Property(p => p.OrganisationClassificationTypeName)
                .HasMaxLength(OrganisationClassificationTypeListConfiguration.NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.OrganisationClassificationTypeName);
            b.HasIndex(x => x.Order);
            b.HasIndex(x => x.Active);

            b.HasIndex(x => new { x.Name, x.OrganisationClassificationTypeId }).IsUnique();
        }
    }

    public class OrganisationClassificationListView :
        Projection<OrganisationClassificationListView>,
        IEventHandler<OrganisationClassificationCreated>,
        IEventHandler<OrganisationClassificationUpdated>,
        IEventHandler<OrganisationClassificationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationClassificationList
        }

        private readonly IEventStore _eventStore;

        public OrganisationClassificationListView(
            ILogger<OrganisationClassificationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationClassification = new OrganisationClassificationListItem
                {
                    Id = message.Body.OrganisationClassificationId,
                    Name = message.Body.Name,
                    Order = message.Body.Order,
                    ExternalKey = message.Body.ExternalKey,
                    Active = message.Body.Active,
                    OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId,
                    OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName
                };

                await context.OrganisationClassificationList.AddAsync(organisationClassification);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationClassification = context.OrganisationClassificationList.SingleOrDefault(x => x.Id == message.Body.OrganisationClassificationId);
                if (organisationClassification == null)
                    return; // TODO: Error?

                organisationClassification.Name = message.Body.Name;
                organisationClassification.Order = message.Body.Order;
                organisationClassification.ExternalKey = message.Body.ExternalKey;
                organisationClassification.Active = message.Body.Active;
                organisationClassification.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;
                organisationClassification.OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationClassifications = context.OrganisationClassificationList.Where(x => x.OrganisationClassificationTypeId == message.Body.OrganisationClassificationTypeId);
                if (!organisationClassifications.Any())
                    return;

                foreach (var organisationClassification in organisationClassifications)
                    organisationClassification.OrganisationClassificationTypeName = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
