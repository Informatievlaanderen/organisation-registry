namespace OrganisationRegistry.SqlServer.Organisation
{
    using System;
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using System.Linq;
    using System.Threading.Tasks;
    using ContactType;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.ContactType.Events;
    using OrganisationRegistry.Infrastructure;

    public class OrganisationContactListItem
    {
        public Guid OrganisationContactId { get; set; }
        public Guid OrganisationId { get; set; }
        public Guid ContactTypeId { get; set; }
        public string ContactTypeName { get; set; }
        public string ContactValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class OrganisationContactListConfiguration : EntityMappingConfiguration<OrganisationContactListItem>
    {
        public const int ContactValueLength = 500;

        public override void Map(EntityTypeBuilder<OrganisationContactListItem> b)
        {
            b.ToTable(nameof(OrganisationContactListView.ProjectionTables.OrganisationContactList), WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.OrganisationContactId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

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

    public class OrganisationContactListView :
        Projection<OrganisationContactListView>,
        IEventHandler<OrganisationContactAdded>,
        IEventHandler<OrganisationContactUpdated>,
        IEventHandler<ContactTypeUpdated>,
        IEventHandler<OrganisationTerminated>,
        IEventHandler<OrganisationTerminatedV2>
    {
        protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
        public override string Schema => WellknownSchemas.BackofficeSchema;

        public enum ProjectionTables
        {
            OrganisationContactList
        }

        private readonly IEventStore _eventStore;

        public OrganisationContactListView(
            ILogger<OrganisationContactListView> logger,
            IEventStore eventStore,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var organisationContacts = context.OrganisationContactList.Where(x => x.ContactTypeId == message.Body.ContactTypeId);
            if (!organisationContacts.Any())
                return;

            foreach (var organisationContact in organisationContacts)
                organisationContact.ContactTypeName = message.Body.Name;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactAdded> message)
        {
            var organisationContactListItem = new OrganisationContactListItem
            {
                OrganisationContactId = message.Body.OrganisationContactId,
                OrganisationId = message.Body.OrganisationId,
                ContactTypeId = message.Body.ContactTypeId,
                ContactValue = message.Body.Value,
                ContactTypeName = message.Body.ContactTypeName,
                ValidFrom = message.Body.ValidFrom,
                ValidTo = message.Body.ValidTo
            };

            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            await context.OrganisationContactList.AddAsync(organisationContactListItem);
            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactUpdated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var contact = context.OrganisationContactList.SingleOrDefault(item => item.OrganisationContactId == message.Body.OrganisationContactId);

            contact.OrganisationContactId = message.Body.OrganisationContactId;
            contact.OrganisationId = message.Body.OrganisationId;
            contact.ContactTypeId = message.Body.ContactTypeId;
            contact.ContactValue = message.Body.Value;
            contact.ContactTypeName = message.Body.ContactTypeName;
            contact.ValidFrom = message.Body.ValidFrom;
            contact.ValidTo = message.Body.ValidTo;

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var contacts = context.OrganisationContactList.Where(item =>
                message.Body.FieldsToTerminate.Contacts.Keys.Contains(item.OrganisationContactId));

            foreach (var contact in contacts)
                contact.ValidTo = message.Body.FieldsToTerminate.Contacts[contact.OrganisationContactId];

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
            var contacts = context.OrganisationContactList.Where(item =>
                message.Body.FieldsToTerminate.Contacts.Keys.Contains(item.OrganisationContactId));

            foreach (var contact in contacts)
                contact.ValidTo = message.Body.FieldsToTerminate.Contacts[contact.OrganisationContactId];

            await context.SaveChangesAsync();
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
