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
    using LabelType;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.LabelType.Events;

    public class OrganisationLabelListItem
    {
        public Guid OrganisationLabelId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid LabelTypeId { get; set; }
        public string LabelTypeName { get; set; }
        public string LabelValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public string? Source { get; set; }

        public bool IsEditable => Source != Sources.Kbo;
    }

    public class OrganisationLabelListConfiguration : EntityMappingConfiguration<OrganisationLabelListItem>
    {
        public const int LabelValueLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationLabelListItem> b)
        {
            b.ToTable(nameof(OrganisationLabelListView.ProjectionTables.OrganisationLabelList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationLabelId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.LabelTypeId).IsRequired();
            b.Property(p => p.LabelTypeName).HasMaxLength(LabelTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.LabelValue).HasMaxLength(LabelValueLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.Source);

            b.HasIndex(x => x.LabelTypeName).IsClustered();
            b.HasIndex(x => x.LabelValue);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationLabelListView :
        Projection<OrganisationLabelListView>,
        IEventHandler<OrganisationLabelAdded>,
        IEventHandler<KboFormalNameLabelAdded>,
        IEventHandler<KboFormalNameLabelRemoved>,
        IEventHandler<OrganisationLabelUpdated>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminationSyncedWithKbo>,
        IEventHandler<LabelTypeUpdated>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationLabelList
        }

        private readonly IEventStore _eventStore;
        public OrganisationLabelListView(
            ILogger<OrganisationLabelListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationLabels = context.OrganisationLabelList.Where(x => x.LabelTypeId == message.Body.LabelTypeId);
            if (!organisationLabels.Any())
                return;

            foreach (var organisationLable in organisationLabels)
                organisationLable.LabelTypeName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelAdded> message)
        {
            var organisationLabelListItem = new OrganisationLabelListItem
            {
                OrganisationLabelId = message.Body.OrganisationLabelId,
                OrganisationId = message.Body.OrganisationId,
                LabelTypeId = message.Body.LabelTypeId,
                LabelValue = message.Body.Value,
                LabelTypeName = message.Body.LabelTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationLabelList.AddAsync(organisationLabelListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelAdded> message)
        {
            var organisationLabelListItem = new OrganisationLabelListItem
            {
                OrganisationLabelId = message.Body.OrganisationLabelId,
                OrganisationId = message.Body.OrganisationId,
                LabelTypeId = message.Body.LabelTypeId,
                LabelValue = message.Body.Value,
                LabelTypeName = message.Body.LabelTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            organisationLabelListItem.Source = Sources.Kbo;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationLabelList.AddAsync(organisationLabelListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (message.Body.FormalNameOrganisationLabelIdToCancel == null)
                return;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var formalNameLabel = await context.OrganisationLabelList
                .SingleAsync(item => item.OrganisationLabelId == message.Body.FormalNameOrganisationLabelIdToCancel);

            context.OrganisationLabelList.Remove(formalNameLabel);

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (message.Body.FormalNameOrganisationLabelIdToTerminate == null)
                return;

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var formalNameLabel = await context.OrganisationLabelList
                .SingleAsync(item => item.OrganisationLabelId == message.Body.FormalNameOrganisationLabelIdToTerminate);

            formalNameLabel.ValidTo = message.Body.DateOfTermination;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelRemoved> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var label = context.OrganisationLabelList.SingleOrDefault(item => item.OrganisationLabelId == message.Body.OrganisationLabelId);

            context.OrganisationLabelList.Remove(label);

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var label = context.OrganisationLabelList.SingleOrDefault(item => item.OrganisationLabelId == message.Body.OrganisationLabelId);

            label.OrganisationLabelId = message.Body.OrganisationLabelId;
            label.OrganisationId = message.Body.OrganisationId;
            label.LabelTypeId = message.Body.LabelTypeId;
            label.LabelValue = message.Body.Value;
            label.LabelTypeName = message.Body.LabelTypeName;
            label.ValidFrom = message.Body.ValidFrom;
            label.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var labels = context.OrganisationLabelList.Where(item =>
                message.Body.FieldsToTerminate.Labels.Keys.Contains(item.OrganisationLabelId));

            foreach (var label in labels)
                label.ValidTo = message.Body.FieldsToTerminate.Labels[label.OrganisationLabelId];

            if (message.Body.KboFieldsToTerminate.FormalName.HasValue)
            {
                var kboFormalName =
                    await context.OrganisationLabelList.SingleAsync(item =>
                        message.Body.KboFieldsToTerminate.FormalName.Value.Key == item.OrganisationLabelId);

                kboFormalName.ValidTo = message.Body.KboFieldsToTerminate.FormalName.Value.Value;
            }

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var labels = context.OrganisationLabelList.Where(item =>
                message.Body.FieldsToTerminate.Labels.Keys.Contains(item.OrganisationLabelId));

            foreach (var label in labels)
                label.ValidTo = message.Body.FieldsToTerminate.Labels[label.OrganisationLabelId];

            if (message.Body.KboFieldsToTerminate.FormalName.HasValue)
            {
                var kboFormalName =
                    await context.OrganisationLabelList.SingleAsync(item =>
                        message.Body.KboFieldsToTerminate.FormalName.Value.Key == item.OrganisationLabelId);

                kboFormalName.ValidTo = message.Body.KboFieldsToTerminate.FormalName.Value.Value;
            }

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
