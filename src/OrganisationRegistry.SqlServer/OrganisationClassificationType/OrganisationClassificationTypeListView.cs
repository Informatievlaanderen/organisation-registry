namespace OrganisationRegistry.SqlServer.OrganisationClassificationType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.OrganisationClassificationType.Events;

    public class OrganisationClassificationTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class OrganisationClassificationTypeListConfiguration : EntityMappingConfiguration<OrganisationClassificationTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationClassificationTypeListItem> b)
        {
            b.ToTable(nameof(OrganisationClassificationTypeListView.ProjectionTables.OrganisationClassificationTypeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class OrganisationClassificationTypeListView :
        Projection<OrganisationClassificationTypeListView>,
        IEventHandler<OrganisationClassificationTypeCreated>,
        IEventHandler<OrganisationClassificationTypeUpdated>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            OrganisationClassificationTypeList
        }

        private readonly IEventStore _eventStore;

        public OrganisationClassificationTypeListView(
            ILogger<OrganisationClassificationTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationClassificationType = new OrganisationClassificationTypeListItem
                {
                    Id = message.Body.OrganisationClassificationTypeId,
                    Name = message.Body.Name,
                };

                await context.OrganisationClassificationTypeList.AddAsync(organisationClassificationType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var organisationClassificationType = context.OrganisationClassificationTypeList.SingleOrDefault(x => x.Id == message.Body.OrganisationClassificationTypeId);
                if (organisationClassificationType == null)
                    return; // TODO: Error?

                organisationClassificationType.Name = message.Body.Name;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
