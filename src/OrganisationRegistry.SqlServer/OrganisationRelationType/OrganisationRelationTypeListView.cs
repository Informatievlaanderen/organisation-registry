namespace OrganisationRegistry.SqlServer.OrganisationRelationType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.OrganisationRelationType.Events;

    public class OrganisationRelationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string InverseName { get; set; }
    }

    public class OrganisationRelationTypeListConfiguration : EntityMappingConfiguration<OrganisationRelationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationRelationTypeListItem> b)
        {
            b.ToTable(nameof(OrganisationRelationTypeListView.ProjectionTables.OrganisationRelationTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.Property(p => p.InverseName)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class OrganisationRelationTypeListView :
        Projection<OrganisationRelationTypeListView>,
        IEventHandler<OrganisationRelationTypeCreated>,
        IEventHandler<OrganisationRelationTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            OrganisationRelationTypeList
        }

        private readonly IEventStore _eventStore;

        public OrganisationRelationTypeListView(
            ILogger<OrganisationRelationTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationRelationType = new OrganisationRelationTypeListItem
                {
                    Id = message.Body.OrganisationRelationTypeId,
                    Name = message.Body.Name,
                    InverseName = message.Body.InverseName
                };

                await context.OrganisationRelationTypeList.AddAsync(organisationRelationType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationRelationType = context.OrganisationRelationTypeList.SingleOrDefault(x => x.Id == message.Body.OrganisationRelationTypeId);
                if (organisationRelationType == null)
                    return; // TODO: Error?

                organisationRelationType.Name = message.Body.Name;
                organisationRelationType.InverseName = message.Body.InverseName;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
