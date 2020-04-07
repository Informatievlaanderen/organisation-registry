namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRelationType;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.OrganisationRelationType.Events;

    public class OrganisationRelationListItem
    {
        public Guid OrganisationRelationId { get; set; }
        public Guid OrganisationId { get; set; }

        public Guid RelationId { get; set; }
        public string RelationName { get; set; }
        public string RelationInverseName { get; set; }

        public Guid RelatedOrganisationId { get; set; }
        public string RelatedOrganisationName { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationRelationListConfiguration : EntityMappingConfiguration<OrganisationRelationListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationRelationListItem> b)
        {
            b.ToTable(nameof(OrganisationRelationListView.ProjectionTables.OrganisationRelationList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationRelationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.RelatedOrganisationId).IsRequired();
            b.Property(p => p.RelatedOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.RelationId).IsRequired();
            b.Property(p => p.RelationName).HasMaxLength(OrganisationRelationTypeListConfiguration.NameLength).IsRequired();
            b.Property(p => p.RelationInverseName).HasMaxLength(OrganisationRelationTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.RelationName);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class OrganisationRelationListView :
        Projection<OrganisationRelationListView>,
        IEventHandler<OrganisationRelationAdded>,
        IEventHandler<OrganisationRelationUpdated>,
        IEventHandler<OrganisationRelationTypeUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationRelationList
        }

        private readonly IEventStore _eventStore;
        private readonly IMemoryCaches _memoryCaches;

        public OrganisationRelationListView(
            ILogger<OrganisationRelationListView> logger,
            IEventStore eventStore,
            IMemoryCaches memoryCaches,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
            _memoryCaches = memoryCaches;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationRelations = context.OrganisationRelationList.Where(x => x.RelationId == message.Body.OrganisationRelationTypeId);
                if (!organisationRelations.Any())
                    return;

                foreach (var organisationRelation in organisationRelations)
                {
                    organisationRelation.RelationName = message.Body.Name;
                    organisationRelation.RelationInverseName = message.Body.InverseName;
                }

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationAdded> message)
        {
            var organisationRelationListItem = new OrganisationRelationListItem
            {
                OrganisationRelationId = message.Body.OrganisationRelationId,
                OrganisationId = message.Body.OrganisationId,

                RelatedOrganisationId = message.Body.RelatedOrganisationId,
                RelatedOrganisationName = _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId],

                RelationId = message.Body.RelationId,
                RelationName = message.Body.RelationName,
                RelationInverseName = message.Body.RelationInverseName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                context.OrganisationRelationList.Add(organisationRelationListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var key = context.OrganisationRelationList.SingleOrDefault(item => item.OrganisationRelationId == message.Body.OrganisationRelationId);
                if (key == null)
                    return;

                key.OrganisationRelationId = message.Body.OrganisationRelationId;
                key.OrganisationId = message.Body.OrganisationId;

                key.RelatedOrganisationId = message.Body.RelatedOrganisationId;
                key.RelatedOrganisationName = _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId];

                key.RelationId = message.Body.RelationId;
                key.RelationName = message.Body.RelationName;
                key.RelationInverseName = message.Body.RelationInverseName;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        }

        private static void UpdateRelatedOrganisationName(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IContextFactory contextFactory,
            Guid organisationId,
            string organisationName)
        {
            using (var context = contextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationRelations =
                    context.OrganisationRelationList.Where(x => x.RelatedOrganisationId == organisationId);
                if (!organisationRelations.Any())
                    return;

                foreach (var organisationRelation in organisationRelations)
                    organisationRelation.RelatedOrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
