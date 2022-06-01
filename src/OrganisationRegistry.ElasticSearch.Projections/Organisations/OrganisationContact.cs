namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System.Data.Common;
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
        return await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Contacts.Single().ContactTypeId, message.Body.ContactTypeId,
                    "contacts", "contactTypeId",
                    "contactTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp))
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactAdded> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
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
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationContactUpdated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
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
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
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
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
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
        ).ToAsyncResult();
    }
}
