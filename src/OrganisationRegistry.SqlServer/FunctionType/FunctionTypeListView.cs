namespace OrganisationRegistry.SqlServer.FunctionType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Function.Events;
    using OrganisationRegistry.Infrastructure;

    public class FunctionTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class FunctionTypeListConfiguration : EntityMappingConfiguration<FunctionTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<FunctionTypeListItem> b)
        {
            b.ToTable(nameof(FunctionTypeListView.ProjectionTables.FunctionList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class FunctionTypeListView :
        Projection<FunctionTypeListView>,
        IEventHandler<FunctionCreated>,
        IEventHandler<FunctionUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            FunctionList
        }

        private readonly IEventStore _eventStore;

        public FunctionTypeListView(
            ILogger<FunctionTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionCreated> message)
        {
            var functionType = new FunctionTypeListItem
            {
                Id = message.Body.FunctionId,
                Name = message.Body.Name,
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.FunctionTypeList.AddAsync(functionType);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var function = await context.FunctionTypeList.SingleOrDefaultAsync(x => x.Id == message.Body.FunctionId);
                if (function == null)
                    return; // TODO: Error?

                function.Name = message.Body.Name;
                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
