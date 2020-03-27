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

        public string Source { get; set; }

        public bool IsEditable => Source != Sources.Kbo;
    }

    public class OrganisationLabelListConfiguration : EntityMappingConfiguration<OrganisationLabelListItem>
    {
        public const int LabelValueLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationLabelListItem> b)
        {
            b.ToTable(nameof(OrganisationLabelListView.ProjectionTables.OrganisationLabelList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationLabelId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.LabelTypeId).IsRequired();
            b.Property(p => p.LabelTypeName).HasMaxLength(LabelTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.LabelValue).HasMaxLength(LabelValueLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.Source);

            b.HasIndex(x => x.LabelTypeName).ForSqlServerIsClustered();
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
        IEventHandler<OrganisationCoupledWithKbo>,
        IEventHandler<LabelTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public static readonly Guid ArchivedNameLabelTypeId = Guid.Parse("00000000-0000-4000-0000-AAA0BBB0CCC0");
        public const string ArchivedNameLabelTypeName = "Gearchiveerde naam uit organisatieregister";

        public enum ProjectionTables
        {
            OrganisationLabelList
        }

        private readonly IEventStore _eventStore;

        public OrganisationLabelListView(
            ILogger<OrganisationLabelListView> logger,
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var organisationLabels = context.OrganisationLabelList.Where(x => x.LabelTypeId == message.Body.LabelTypeId);
                if (!organisationLabels.Any())
                    return;

                foreach (var organisationLable in organisationLabels)
                    organisationLable.LabelTypeName = message.Name;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelAdded> message)
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

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationLabelList.Add(organisationLabelListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelAdded> message)
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

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationLabelList.Add(organisationLabelListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCoupledWithKbo> message)
        {
            var organisationLabelListItem = new OrganisationLabelListItem
            {
                OrganisationLabelId = message.Body.OrganisationId,
                OrganisationId = message.Body.OrganisationId,
                LabelTypeId = ArchivedNameLabelTypeId,
                LabelValue = message.Body.Name,
                LabelTypeName = ArchivedNameLabelTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = null
            };

            organisationLabelListItem.Source = Sources.Kbo;

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.OrganisationLabelList.Add(organisationLabelListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelRemoved> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var label = context.OrganisationLabelList.SingleOrDefault(item => item.OrganisationLabelId == message.Body.OrganisationLabelId);

                context.OrganisationLabelList.Remove(label);

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var label = context.OrganisationLabelList.SingleOrDefault(item => item.OrganisationLabelId == message.Body.OrganisationLabelId);

                label.OrganisationLabelId = message.Body.OrganisationLabelId;
                label.OrganisationId = message.Body.OrganisationId;
                label.LabelTypeId = message.Body.LabelTypeId;
                label.LabelValue = message.Body.Value;
                label.LabelTypeName = message.Body.LabelTypeName;
                label.ValidFrom = message.Body.ValidFrom;
                label.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
