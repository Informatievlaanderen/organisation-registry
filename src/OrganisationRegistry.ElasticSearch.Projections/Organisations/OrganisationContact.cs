namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using ContactType.Events;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;
    using Infrastructure.Change;

    public class OrganisationContact :
        BaseProjection<OrganisationContact>,
        IElasticEventHandler<OrganisationContactAdded>,
        IElasticEventHandler<OrganisationContactUpdated>,
        IElasticEventHandler<ContactTypeUpdated>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {

        public OrganisationContact(
            ILogger<OrganisationContact> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ContactTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Contacts.Single().ContactTypeId, message.Body.ContactTypeId,
                        "contacts", "contactTypeId",
                        "contactTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Contacts == null)
                        document.Contacts = new List<OrganisationDocument.OrganisationContact>();

                    document.Contacts.RemoveExistingListItems(x => x.OrganisationContactId == message.Body.OrganisationContactId);

                    document.Contacts.Add(
                        new OrganisationDocument.OrganisationContact(
                            message.Body.OrganisationContactId,
                            message.Body.ContactTypeId,
                            message.Body.ContactTypeName,
                            message.Body.Value,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Contacts.RemoveExistingListItems(x => x.OrganisationContactId == message.Body.OrganisationContactId);

                    document.Contacts.Add(
                        new OrganisationDocument.OrganisationContact(
                            message.Body.OrganisationContactId,
                            message.Body.ContactTypeId,
                            message.Body.ContactTypeName,
                            message.Body.Value,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Contacts)
                    {
                        var organisationContact =
                            document
                                .Contacts
                                .Single(x => x.OrganisationContactId == key);

                        organisationContact.Validity.End = value;
                    }
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Contacts)
                    {
                        var organisationContact =
                            document
                                .Contacts
                                .Single(x => x.OrganisationContactId == key);

                        organisationContact.Validity.End = value;
                    }
                }
            );
        }
    }
}
