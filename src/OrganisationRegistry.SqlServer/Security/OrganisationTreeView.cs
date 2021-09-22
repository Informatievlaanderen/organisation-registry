namespace OrganisationRegistry.SqlServer.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Security;

    public class OrganisationTreeItem
    {
        public string OvoNumber { get; set; }

        public string? OrganisationTree { get; set; }
    }

    public class OrganisationTreeListConfiguration : EntityMappingConfiguration<OrganisationTreeItem>
    {
        public override void Map(EntityTypeBuilder<OrganisationTreeItem> b)
        {
            b.ToTable(nameof(OrganisationTreeView.ProjectionTables.OrganisationTreeList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.OvoNumber);

            b.Property(p => p.OvoNumber).HasMaxLength(OrganisationListConfiguration.OvoNumberLength);

            b.Property(p => p.OrganisationTree);
        }
    }

    public class OrganisationTreeView :
        Projection<OrganisationTreeView>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<ParentAssignedToOrganisation>,
        IEventHandler<ParentClearedFromOrganisation>,
        IEventHandler<Rollback>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationTreeList
        }

        private readonly IMemoryCaches _memoryCaches;
        private readonly IEventStore _eventStore;
        private ITree<OvoNumber> _tree;

        private class OvoNumber : INodeValue
        {
            public string Id { get; }

            public OvoNumber(string ovoNumber)
            {
                Id = ovoNumber;
            }
        }

        public OrganisationTreeView(
            ILogger<OrganisationTreeView> logger,
            IMemoryCaches memoryCaches,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _memoryCaches = memoryCaches;
            _eventStore = eventStore;

            Initialise();
        }

        private void Initialise()
        {
            _tree = BuildInitialTree(_memoryCaches.OrganisationParents, _memoryCaches.OvoNumbers);
        }

        private static ITree<OvoNumber> BuildInitialTree(IReadOnlyDictionary<Guid, Guid?> organisationParents, IReadOnlyDictionary<Guid, string> organisationOvoNumbers)
        {
            var tree = new Tree<OvoNumber>();

            // Start by just adding all orgs
            foreach (var organisationParent in organisationParents)
                tree.AddNode(new OvoNumber(organisationOvoNumbers[organisationParent.Key]));

            // And then link up their parents
            foreach (var organisationParent in organisationParents.Where(x => x.Value.HasValue))
                tree.ChangeNodeParent(new OvoNumber(organisationOvoNumbers[organisationParent.Key]), new OvoNumber(organisationOvoNumbers[organisationParent.Value.Value]));

            return tree;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            _tree.AddNode(new OvoNumber(message.Body.OvoNumber));
            var changes = _tree.GetChanges().ToList();
            _tree.AcceptChanges();

            UpdateChanges(dbConnection, dbTransaction, ContextFactory, changes);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            _tree.AddNode(new OvoNumber(message.Body.OvoNumber));
            var changes = _tree.GetChanges().ToList();
            _tree.AcceptChanges();

            UpdateChanges(dbConnection, dbTransaction, ContextFactory, changes);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
        {
            var ovoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId];
            var parentOvoNumber = _memoryCaches.OvoNumbers[message.Body.ParentOrganisationId];

            _tree.ChangeNodeParent(new OvoNumber(ovoNumber), new OvoNumber(parentOvoNumber));
            var changes = _tree.GetChanges().ToList();
            _tree.AcceptChanges();

            UpdateChanges(dbConnection, dbTransaction, ContextFactory, changes);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentClearedFromOrganisation> message)
        {
            var ovoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId];

            _tree.RemoveNodeParent(new OvoNumber(ovoNumber));
            var changes = _tree.GetChanges().ToList();
            _tree.AcceptChanges();

            UpdateChanges(dbConnection, dbTransaction, ContextFactory, changes);
        }

        private static void UpdateChanges(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IContextFactory contextFactory,
            IEnumerable<INode<OvoNumber>> changes)
        {
            using (var context = contextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                foreach (var change in changes)
                {
                    var treeItem = context.OrganisationTreeList.SingleOrDefault(x => x.OvoNumber == change.Id);
                    if (treeItem != null)
                    {
                        treeItem.OrganisationTree = change.Traverse().ToSeparatedList();
                    }
                    else
                    {
                        context.Add(new OrganisationTreeItem
                        {
                            OvoNumber = change.Id,
                            OrganisationTree = change.Traverse().ToSeparatedList()
                        });
                    }
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<Rollback> message)
        {
            // Something went wrong, check if any of the events actually matter to us
            var anyInterestingEvents = message.Body.Events.Any(x =>
                x is OrganisationCreated ||
                x is ParentAssignedToOrganisation ||
                x is ParentClearedFromOrganisation);

            if (anyInterestingEvents)
                Initialise();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message, _ => _tree = new Tree<OvoNumber>());
        }
    }
}
