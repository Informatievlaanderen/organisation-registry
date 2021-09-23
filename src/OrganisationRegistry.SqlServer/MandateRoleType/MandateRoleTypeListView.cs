namespace OrganisationRegistry.SqlServer.MandateRoleType
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
    using OrganisationRegistry.MandateRoleType.Events;

    public class MandateRoleTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class MandateRoleTypeListConfiguration : EntityMappingConfiguration<MandateRoleTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<MandateRoleTypeListItem> b)
        {
            b.ToTable(nameof(MandateRoleTypeListView.ProjectionTables.MandateRoleTypeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class MandateRoleTypeListView :
        Projection<MandateRoleTypeListView>,
        IEventHandler<MandateRoleTypeCreated>,
        IEventHandler<MandateRoleTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            MandateRoleTypeList
        }

        private readonly IEventStore _eventStore;

        public MandateRoleTypeListView(
            ILogger<MandateRoleTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MandateRoleTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var mandateRoleType = new MandateRoleTypeListItem
                {
                    Id = message.Body.MandateRoleTypeId,
                    Name = message.Body.Name,
                };

                await context.MandateRoleTypeList.AddAsync(mandateRoleType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MandateRoleTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var mandateRoleType = context.MandateRoleTypeList.SingleOrDefault(x => x.Id == message.Body.MandateRoleTypeId);
                if (mandateRoleType == null)
                    return; // TODO: Error?

                mandateRoleType.Name = message.Body.Name;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
