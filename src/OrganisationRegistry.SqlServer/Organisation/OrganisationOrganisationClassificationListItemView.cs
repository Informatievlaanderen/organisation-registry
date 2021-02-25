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
    using OrganisationClassification;
    using OrganisationClassificationType;
    using OrganisationRegistry.OrganisationClassificationType.Events;
    using OrganisationRegistry.OrganisationClassification.Events;

    public class OrganisationOrganisationClassificationListItem
    {
        public Guid OrganisationOrganisationClassificationId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid OrganisationClassificationTypeId { get; set; }
        public string OrganisationClassificationTypeName { get; set; }

        public Guid OrganisationClassificationId { get; set; }
        public string OrganisationClassificationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public string? Source { get; set; }

        public bool IsEditable => Source != Sources.Kbo;
    }

    public class OrganisationOrganisationClassificationListConfiguration : EntityMappingConfiguration<OrganisationOrganisationClassificationListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationOrganisationClassificationListItem> b)
        {
            b.ToTable(nameof(OrganisationOrganisationClassificationListView.ProjectionTables.OrganisationOrganisationClassificationList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationOrganisationClassificationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.OrganisationClassificationTypeId).IsRequired();
            b.Property(p => p.OrganisationClassificationTypeName).HasMaxLength(OrganisationClassificationTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.OrganisationClassificationId).IsRequired();
            b.Property(p => p.OrganisationClassificationName).HasMaxLength(OrganisationClassificationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.Source);

            b.HasIndex(x => x.OrganisationClassificationTypeName).IsClustered();
            b.HasIndex(x => x.OrganisationClassificationName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationOrganisationClassificationListView :
        Projection<OrganisationOrganisationClassificationListView>,
        IEventHandler<OrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminationSyncedWithKbo>,
        IEventHandler<OrganisationOrganisationClassificationUpdated>,
        IEventHandler<OrganisationClassificationTypeUpdated>,
        IEventHandler<OrganisationClassificationUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationOrganisationClassificationList
        }

        private readonly IEventStore _eventStore;

        public OrganisationOrganisationClassificationListView(
            ILogger<OrganisationOrganisationClassificationListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationOrganisationClassifications =
                    context.OrganisationOrganisationClassificationList
                        .Where(x => x.OrganisationClassificationTypeId == message.Body.OrganisationClassificationTypeId);

                if (!organisationOrganisationClassifications.Any())
                    return;

                foreach (var organisationOrganisationClassification in organisationOrganisationClassifications)
                    organisationOrganisationClassification.OrganisationClassificationTypeName = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationOrganisationClassifications =
                    context.OrganisationOrganisationClassificationList.Where(x =>
                        x.OrganisationClassificationId == message.Body.OrganisationClassificationId);

                if (!organisationOrganisationClassifications.Any())
                    return;

                foreach (var organisationOrganisationClassification in organisationOrganisationClassifications)
                {
                    organisationOrganisationClassification.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;
                    organisationOrganisationClassification.OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName;
                    organisationOrganisationClassification.OrganisationClassificationName = message.Body.Name;
                }

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
        {
            var organisationOrganisationClassificationListItem = new OrganisationOrganisationClassificationListItem
            {
                OrganisationOrganisationClassificationId = message.Body.OrganisationOrganisationClassificationId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId,
                OrganisationClassificationId = message.Body.OrganisationClassificationId,
                OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName,
                OrganisationClassificationName = message.Body.OrganisationClassificationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationOrganisationClassificationList.AddAsync(organisationOrganisationClassificationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
        {
            var organisationOrganisationClassificationListItem = new OrganisationOrganisationClassificationListItem
            {
                OrganisationOrganisationClassificationId = message.Body.OrganisationOrganisationClassificationId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId,
                OrganisationClassificationId = message.Body.OrganisationClassificationId,
                OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName,
                OrganisationClassificationName = message.Body.OrganisationClassificationName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            organisationOrganisationClassificationListItem.Source = Sources.Kbo;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.OrganisationOrganisationClassificationList.AddAsync(organisationOrganisationClassificationListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var key = context.OrganisationOrganisationClassificationList.Single(item => item.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

                context.OrganisationOrganisationClassificationList.Remove(key);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel == null)
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationLocationListItem = context.OrganisationOrganisationClassificationList.Single(b =>
                    b.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel.Value);

                context.OrganisationOrganisationClassificationList.Remove(organisationLocationListItem);

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate == null)
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var legalFormOrganisationClassification = context.OrganisationOrganisationClassificationList.Single(b =>
                    b.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate);

                legalFormOrganisationClassification.ValidTo = message.Body.DateOfTermination;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var key = context.OrganisationOrganisationClassificationList.SingleOrDefault(item => item.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

                key.OrganisationOrganisationClassificationId = message.Body.OrganisationOrganisationClassificationId;
                key.OrganisationId = message.Body.OrganisationId;
                key.OrganisationClassificationTypeId = message.Body.OrganisationClassificationTypeId;
                key.OrganisationClassificationId = message.Body.OrganisationClassificationId;
                key.OrganisationClassificationTypeName = message.Body.OrganisationClassificationTypeName;
                key.OrganisationClassificationName = message.Body.OrganisationClassificationName;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var classifications = context.OrganisationOrganisationClassificationList.Where(item =>
                    message.Body.FieldsToTerminate.Classifications.Keys.Contains(item.OrganisationOrganisationClassificationId));

                foreach (var classification in classifications)
                {
                    classification.ValidTo = message.Body.FieldsToTerminate.Classifications[classification.OrganisationOrganisationClassificationId];
                }

                if (message.Body.KboFieldsToTerminate.LegalForm.HasValue)
                {
                    var kboLegalForm =
                        await context.OrganisationOrganisationClassificationList.SingleAsync(item =>
                            message.Body.KboFieldsToTerminate.LegalForm.Value.Key == item.OrganisationOrganisationClassificationId);

                    kboLegalForm.ValidTo = message.Body.KboFieldsToTerminate.LegalForm.Value.Value;
                }

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
