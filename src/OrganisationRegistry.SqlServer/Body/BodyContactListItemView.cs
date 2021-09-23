namespace OrganisationRegistry.SqlServer.Body
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Body.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using ContactType;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.ContactType.Events;
    using OrganisationRegistry.Infrastructure;

    public class BodyContactListItem
    {
        public Guid BodyContactId { get; set; }
        public Guid BodyId { get; set; }
        public Guid ContactTypeId { get; set; }
        public string ContactTypeName { get; set; }
        public string ContactValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class BodyContactListConfiguration : EntityMappingConfiguration<BodyContactListItem>
    {
        public const int ContactValueLength = 500;

        public override void Map(EntityTypeBuilder<BodyContactListItem> b)
        {
            b.ToTable(nameof(BodyContactListView.ProjectionTables.BodyContactList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.BodyContactId)
                .IsClustered(false);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.ContactTypeId).IsRequired();
            b.Property(p => p.ContactTypeName).HasMaxLength(ContactTypeListConfiguration.NameLength).IsRequired();

            b.Property(p => p.ContactValue).HasMaxLength(ContactValueLength).IsRequired();

            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ContactTypeName).IsClustered();
            b.HasIndex(x => x.ContactValue);
            b.HasIndex(x => x.ValidFrom);
            b.HasIndex(x => x.ValidTo);
        }
    }

    public class BodyContactListView :
        Projection<BodyContactListView>,
        IEventHandler<BodyContactAdded>,
        IEventHandler<BodyContactUpdated>,
        IEventHandler<ContactTypeUpdated>
    {
        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            BodyContactList
        }

        private readonly IEventStore _eventStore;

        public BodyContactListView(
            ILogger<BodyContactListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var bodyContacts = context.BodyContactList.Where(x => x.ContactTypeId == message.Body.ContactTypeId);
                if (!bodyContacts.Any())
                    return;

                foreach (var organisationContact in bodyContacts)
                    organisationContact.ContactTypeName = message.Body.Name;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyContactAdded> message)
        {
            var organisationContactListItem = new BodyContactListItem
            {
                BodyContactId = message.Body.BodyContactId,
                BodyId = message.Body.BodyId,
                ContactTypeId = message.Body.ContactTypeId,
                ContactValue = message.Body.Value,
                ContactTypeName = message.Body.ContactTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.BodyContactList.AddAsync(organisationContactListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyContactUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var contact = context.BodyContactList.SingleOrDefault(item => item.BodyContactId == message.Body.BodyContactId);

                contact.BodyContactId = message.Body.BodyContactId;
                contact.BodyId = message.Body.BodyId;
                contact.ContactTypeId = message.Body.ContactTypeId;
                contact.ContactValue = message.Body.Value;
                contact.ContactTypeName = message.Body.ContactTypeName;
                contact.ValidFrom = message.Body.ValidFrom;
                contact.ValidTo = message.Body.ValidTo;

                await context.SaveChangesAsync();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
