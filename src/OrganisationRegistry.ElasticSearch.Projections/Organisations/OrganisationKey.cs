namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Infrastructure;
    using KeyTypes.Events;
    using Microsoft.Extensions.Logging;
    using Common;
    using Infrastructure.Change;

    public class OrganisationKey :
        BaseProjection<OrganisationKey>,
        IElasticEventHandler<OrganisationKeyAdded>,
        IElasticEventHandler<OrganisationKeyUpdated>,
        IElasticEventHandler<KeyTypeUpdated>
    {
        public OrganisationKey(
            ILogger<OrganisationKey> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Keys.Single().KeyTypeId, message.Body.KeyTypeId,
                        "keys", "keyTypeId",
                        "keyTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Keys == null)
                        document.Keys = new List<OrganisationDocument.OrganisationKey>();

                    document.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);

                    document.Keys.Add(
                        new OrganisationDocument.OrganisationKey(
                            message.Body.OrganisationKeyId,
                            message.Body.KeyTypeId,
                            message.Body.KeyTypeName,
                            message.Body.Value,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);

                    document.Keys.Add(
                        new OrganisationDocument.OrganisationKey(
                            message.Body.OrganisationKeyId,
                            message.Body.KeyTypeId,
                            message.Body.KeyTypeName,
                            message.Body.Value,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }
    }
}
