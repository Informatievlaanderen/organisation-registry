namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Infrastructure;
    using KeyTypes.Events;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationKey :
        BaseProjection<OrganisationKey>,
        IEventHandler<OrganisationKeyAdded>,
        IEventHandler<OrganisationKeyUpdated>,
        IEventHandler<KeyTypeUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationKey(
            ILogger<OrganisationKey> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Keys.Single().KeyTypeId, message.Body.KeyTypeId,
                    "keys", "keyTypeId",
                    "keyTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Keys == null)
                organisationDocument.Keys = new List<OrganisationDocument.OrganisationKey>();

            organisationDocument.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);

            organisationDocument.Keys.Add(
                new OrganisationDocument.OrganisationKey(
                    message.Body.OrganisationKeyId,
                    message.Body.KeyTypeId,
                    message.Body.KeyTypeName,
                    message.Body.Value,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);

            organisationDocument.Keys.Add(
                new OrganisationDocument.OrganisationKey(
                    message.Body.OrganisationKeyId,
                    message.Body.KeyTypeId,
                    message.Body.KeyTypeName,
                    message.Body.Value,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
