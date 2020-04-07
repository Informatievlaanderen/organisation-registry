namespace OrganisationRegistry.SqlServer.FormalFrameworkCategory
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.FormalFrameworkCategory.Events;

    public class FormalFrameworkCategoryListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class FormalFrameworkCategoryListConfiguration : EntityMappingConfiguration<FormalFrameworkCategoryListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<FormalFrameworkCategoryListItem> b)
        {
            b.ToTable(nameof(FormalFrameworkCategoryListView.ProjectionTables.FormalFrameworkCategoryList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class FormalFrameworkCategoryListView :
        Projection<FormalFrameworkCategoryListView>,
        IEventHandler<FormalFrameworkCategoryCreated>,
        IEventHandler<FormalFrameworkCategoryUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            FormalFrameworkCategoryList
        }

        private readonly IEventStore _eventStore;
        private Func<DbConnection, DbTransaction, OrganisationRegistryContext> _contextFactory;

        public FormalFrameworkCategoryListView(
            ILogger<FormalFrameworkCategoryListView> logger,
            IEventStore eventStore,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory = null) : base(logger)
        {
            _eventStore = eventStore;
            _contextFactory = contextFactory ?? ((connection, transaction) =>
                new OrganisationRegistryTransactionalContext(connection, transaction));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkCategoryCreated> message)
        {
            var formalFrameworkCategory = new FormalFrameworkCategoryListItem
            {
                Id = message.Body.FormalFrameworkCategoryId,
                Name = message.Body.Name,
            };

            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                context.FormalFrameworkCategoryList.Add(formalFrameworkCategory);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkCategoryUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var formalFrameworkCategory = context.FormalFrameworkCategoryList.SingleOrDefault(x => x.Id == message.Body.FormalFrameworkCategoryId);
                if (formalFrameworkCategory == null)
                    return; // TODO: Error?

                formalFrameworkCategory.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
