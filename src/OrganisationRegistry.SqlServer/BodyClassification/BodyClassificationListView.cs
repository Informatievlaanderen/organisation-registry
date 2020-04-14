namespace OrganisationRegistry.SqlServer.BodyClassification
{
    using BodyClassificationType;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.BodyClassification.Events;
    using OrganisationRegistry.BodyClassificationType.Events;
    using OrganisationRegistry.Infrastructure.Events;

    public class BodyClassificationListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Order { get; set; }

        public bool Active { get; set; }

        public Guid BodyClassificationTypeId { get; set; }
        public string BodyClassificationTypeName { get; set; }
    }

    public class BodyClassificationListConfiguration : EntityMappingConfiguration<BodyClassificationListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<BodyClassificationListItem> b)
        {
            b.ToTable(nameof(BodyClassificationListView.ProjectionTables.BodyClassificationList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.Order).IsRequired();

            b.Property(p => p.Active);

            b.Property(p => p.BodyClassificationTypeId).IsRequired();
            b.Property(p => p.BodyClassificationTypeName)
                .HasMaxLength(BodyClassificationTypeListConfiguration.NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.BodyClassificationTypeName);
            b.HasIndex(x => x.Order);
            b.HasIndex(x => x.Active);

            b.HasIndex(x => new { x.Name, x.BodyClassificationTypeId }).IsUnique();
        }
    }

    public class BodyClassificationListView :
        Projection<BodyClassificationListView>,
        IEventHandler<BodyClassificationCreated>,
        IEventHandler<BodyClassificationUpdated>,
        IEventHandler<BodyClassificationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyClassificationList
        }

        private readonly IEventStore _eventStore;

        public BodyClassificationListView(
            ILogger<BodyClassificationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodyClassification = new BodyClassificationListItem
                {
                    Id = message.Body.BodyClassificationId,
                    Name = message.Body.Name,
                    Order = message.Body.Order,
                    Active = message.Body.Active,
                    BodyClassificationTypeId = message.Body.BodyClassificationTypeId,
                    BodyClassificationTypeName = message.Body.BodyClassificationTypeName
                };

                await context.BodyClassificationList.AddAsync(bodyClassification);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodyClassification = context.BodyClassificationList.SingleOrDefault(x => x.Id == message.Body.BodyClassificationId);
                if (bodyClassification == null)
                    return; // TODO: Error?

                bodyClassification.Name = message.Body.Name;
                bodyClassification.Order = message.Body.Order;
                bodyClassification.Active = message.Body.Active;
                bodyClassification.BodyClassificationTypeId = message.Body.BodyClassificationTypeId;
                bodyClassification.BodyClassificationTypeName = message.Body.BodyClassificationTypeName;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClassificationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodyClassifications = context.BodyClassificationList.Where(x => x.BodyClassificationTypeId == message.Body.BodyClassificationTypeId);
                if (!bodyClassifications.Any())
                    return;

                foreach (var bodyClassification in bodyClassifications)
                    bodyClassification.BodyClassificationTypeName = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
