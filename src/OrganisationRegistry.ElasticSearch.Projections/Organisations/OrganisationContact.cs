namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using ContactType.Events;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationContact :
        BaseProjection<OrganisationContact>,
        IEventHandler<OrganisationContactAdded>,
        IEventHandler<OrganisationContactUpdated>,
        IEventHandler<ContactTypeUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;

        public OrganisationContact(
            ILogger<OrganisationContact> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Contacts.Single().ContactTypeId, message.Body.ContactTypeId,
                    "contacts", "contactTypeId",
                    "contactTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Contacts == null)
                organisationDocument.Contacts = new List<OrganisationDocument.OrganisationContact>();

            organisationDocument.Contacts.RemoveExistingListItems(x => x.OrganisationContactId == message.Body.OrganisationContactId);

            organisationDocument.Contacts.Add(
                new OrganisationDocument.OrganisationContact(
                    message.Body.OrganisationContactId,
                    message.Body.ContactTypeId,
                    message.Body.ContactTypeName,
                    message.Body.Value,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Contacts.RemoveExistingListItems(x => x.OrganisationContactId == message.Body.OrganisationContactId);

            organisationDocument.Contacts.Add(
                new OrganisationDocument.OrganisationContact(
                    message.Body.OrganisationContactId,
                    message.Body.ContactTypeId,
                    message.Body.ContactTypeName,
                    message.Body.Value,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            foreach (var (key, value) in message.Body.ContactsToTerminate)
            {
                var organisationContact =
                    organisationDocument
                        .Contacts
                        .Single(x => x.OrganisationContactId == key);

                organisationContact.Validity.End = value;
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
