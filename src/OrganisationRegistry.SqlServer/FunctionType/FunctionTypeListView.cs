namespace OrganisationRegistry.SqlServer.FunctionType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Function.Events;

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
            b.ToTable(nameof(FunctionTypeListView.ProjectionTables.FunctionList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
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
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionCreated> message)
        {
            var functionType = new FunctionTypeListItem
            {
                Id = message.Body.FunctionId,
                Name = message.Body.Name,
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.FunctionTypeList.Add(functionType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var function = context.FunctionTypeList.SingleOrDefault(x => x.Id == message.Body.FunctionId);
                if (function == null)
                    return; // TODO: Error?

                function.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
