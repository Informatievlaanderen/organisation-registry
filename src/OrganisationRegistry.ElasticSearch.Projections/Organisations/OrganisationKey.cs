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
        IElasticEventHandler<OrganisationKeyRemoved>,
        IElasticEventHandler<KeyTypeUpdated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {
        public OrganisationKey(
            ILogger<OrganisationKey> logger) : base(logger)
        {
        }

        public Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KeyTypeUpdated> message)
            => new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Keys.Single().KeyTypeId, message.Body.KeyTypeId,
                        "keys", "keyTypeId",
                        "keyTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            ).ToAsyncResult<IElasticChange>();

        public Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyAdded> message)
            => new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Keys is not {})
                        document.Keys = new List<OrganisationDocument.OrganisationKey>();

                    document.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);

                    document.Keys.Add(
                        new OrganisationDocument.OrganisationKey(
                            message.Body.OrganisationKeyId,
                            message.Body.KeyTypeId,
                            message.Body.KeyTypeName,
                            message.Body.Value,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            ).ToAsyncResult<IElasticChange>();

        public Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationKeyUpdated> message)
            => new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
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
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            ).ToAsyncResult<IElasticChange>();

        public Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationKeyRemoved> message)
            => new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Keys.RemoveExistingListItems(x => x.OrganisationKeyId == message.Body.OrganisationKeyId);
                }
            ).ToAsyncResult<IElasticChange>();

        public Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationTerminatedV2> message)
        {
            if (message.Body.FieldsToTerminate.Keys == null)
                return new ElasticNoChange().ToAsyncResult<IElasticChange>();

            return new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Keys)
                    {
                        var organisationKey =
                            document
                                .Keys
                                .Single(x => x.OrganisationKeyId == key);

                        organisationKey.Validity.End = value;
                    }
                }
            ).ToAsyncResult<IElasticChange>();
        }
    }
}
