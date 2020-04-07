namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationChildListItem
    {
        public Guid OrganisationOrganisationParentId { get; set; }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OvoNumber { get; set; }

        public Guid ParentOrganisationId { get; set; }

        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public DateTime? OrganisationValidFrom { get; set; }
        public DateTime? OrganisationValidTo { get; set; }
    }

    public class OrganisationChildListConfiguration : EntityMappingConfiguration<OrganisationChildListItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationChildListItem> b)
        {
            b.ToTable(nameof(OrganisationChildListView.ProjectionTables.OrganisationChildList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationOrganisationParentId)
                .IsClustered(false);

            b.Property(p => p.Id).IsRequired();

            b.Property(p => p.Name).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

            b.Property(p => p.OvoNumber).IsRequired();

            b.Property(p => p.ParentOrganisationId).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.Property(p => p.OrganisationValidFrom);
            b.Property(p => p.OrganisationValidTo);

            b.HasIndex(x => x.Name).IsClustered();
            b.HasIndex(x => x.OvoNumber);

            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
            b.HasIndex(x => x.OrganisationValidFrom);
            b.HasIndex(x => x.OrganisationValidTo);
        }
    }

    public class OrganisationChildListView :
        Projection<OrganisationChildListView>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationParentAdded>,
        IEventHandler<OrganisationParentUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationChildList
        }

        private readonly IEventStore _eventStore;
        private readonly IMemoryCaches _memoryCaches;
        private Func<DbConnection, DbTransaction, OrganisationRegistryContext> _contextFactory;

        public OrganisationChildListView(
            ILogger<OrganisationChildListView> logger,
            IEventStore eventStore,
            IMemoryCaches memoryCaches,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory = null) : base(logger)
        {
            _eventStore = eventStore;
            _memoryCaches = memoryCaches;
            _contextFactory = contextFactory ?? ((connection, transaction) =>
                                  new OrganisationRegistryTransactionalContext(connection, transaction));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                {
                    organisation.Name = message.Body.Name;
                    organisation.OvoNumber = message.Body.OvoNumber;
                    organisation.OrganisationValidFrom = message.Body.ValidFrom;
                    organisation.OrganisationValidTo = message.Body.ValidTo;
                }

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
                if (!organisations.Any())
                    return;

                foreach (var organisation in organisations)
                {
                    organisation.Name = message.Body.Name;
                }

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
        {
            var organisationParentListItem = new OrganisationChildListItem
            {
                OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
                Id = message.Body.OrganisationId,
                Name = _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                OvoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId],
                ParentOrganisationId = message.Body.ParentOrganisationId,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo,
                OrganisationValidFrom = _memoryCaches.OrganisationValidFroms[message.Body.OrganisationId],
                OrganisationValidTo = _memoryCaches.OrganisationValidTos[message.Body.OrganisationId]
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.OrganisationChildrenList.Add(organisationParentListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var key = context.OrganisationChildrenList.SingleOrDefault(item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                key.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
                key.Id = message.Body.OrganisationId;
                key.Name = _memoryCaches.OrganisationNames[message.Body.OrganisationId];
                key.OvoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId];
                key.ParentOrganisationId = message.Body.ParentOrganisationId;
                key.ValidFrom = message.Body.ValidFrom;
                key.ValidTo = message.Body.ValidTo;
                key.OrganisationValidFrom = _memoryCaches.OrganisationValidFroms[message.Body.OrganisationId];
                key.OrganisationValidTo = _memoryCaches.OrganisationValidTos[message.Body.OrganisationId];

                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
