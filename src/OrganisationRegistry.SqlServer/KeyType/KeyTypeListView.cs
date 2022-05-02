namespace OrganisationRegistry.SqlServer.KeyType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure;
    using KeyTypes.Events;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;

    public class KeyTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;
        public bool IsRemoved { get; set; }
    }

    public class KeyTypeListConfiguration : EntityMappingConfiguration<KeyTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<KeyTypeListItem> b)
        {
            b.ToTable(nameof(KeyTypeListView.ProjectionTables.KeyTypeList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class KeyTypeListView :
        Projection<KeyTypeListView>,
        IEventHandler<KeyTypeCreated>,
        IEventHandler<KeyTypeUpdated>,
        IEventHandler<KeyTypeRemoved>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            KeyTypeList
        }

        private readonly IEventStore _eventStore;

        public KeyTypeListView(
            ILogger<KeyTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeCreated> message)
        {
            var keyType = new KeyTypeListItem
            {
                Id = message.Body.KeyTypeId,
                Name = message.Body.Name,
                IsRemoved = false,
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

            await context.KeyTypeList.AddAsync(keyType);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

            var keyType = context.KeyTypeList.Single(x => x.Id == message.Body.KeyTypeId);

            keyType.Name = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeRemoved> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

            var keyType = context.KeyTypeList.Single(x => x.Id == message.Body.KeyTypeId);

            keyType.IsRemoved = true;

            await context.SaveChangesAsync();
        }
    }
}
