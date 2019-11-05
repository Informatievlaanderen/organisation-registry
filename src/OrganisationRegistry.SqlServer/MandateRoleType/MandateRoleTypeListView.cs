namespace OrganisationRegistry.SqlServer.MandateRoleType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
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
            b.ToTable(nameof(MandateRoleTypeListView.ProjectionTables.MandateRoleTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().ForSqlServerIsClustered();
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
            IEventStore eventStore) : base(logger)
        {
            _eventStore = eventStore;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MandateRoleTypeCreated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var mandateRoleType = new MandateRoleTypeListItem
                {
                    Id = message.Body.MandateRoleTypeId,
                    Name = message.Body.Name,
                };

                context.MandateRoleTypeList.Add(mandateRoleType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MandateRoleTypeUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var mandateRoleType = context.MandateRoleTypeList.SingleOrDefault(x => x.Id == message.Body.MandateRoleTypeId);
                if (mandateRoleType == null)
                    return; // TODO: Error?

                mandateRoleType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
