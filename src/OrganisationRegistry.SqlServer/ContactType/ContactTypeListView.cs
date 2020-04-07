namespace OrganisationRegistry.SqlServer.ContactType
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.ContactType.Events;

    public class ContactTypeListItem
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class ContactTypeListConfiguration : EntityMappingConfiguration<ContactTypeListItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<ContactTypeListItem> b)
        {
            b.ToTable(nameof(ContactTypeListView.ProjectionTables.ContactTypeList), "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name)
                .HasMaxLength(NameLength)
                .IsRequired();

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }

    public class ContactTypeListView :
        Projection<ContactTypeListView>,
        IEventHandler<ContactTypeCreated>,
        IEventHandler<ContactTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ContactTypeList
        }

        private readonly IEventStore _eventStore;

        public ContactTypeListView(
            ILogger<ContactTypeListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public ContactTypeListView(
            ILogger<ContactTypeListView> logger,
            IContextFactory contextFactory) : base(logger, contextFactory) { }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeCreated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var contactType = new ContactTypeListItem
                {
                    Id = message.Body.ContactTypeId,
                    Name = message.Body.Name,
                };

                context.ContactTypeList.Add(contactType);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var contactType = context.ContactTypeList.SingleOrDefault(x => x.Id == message.Body.ContactTypeId);
                if (contactType == null)
                    return; // TODO: Error?

                contactType.Name = message.Body.Name;
                context.SaveChanges();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
