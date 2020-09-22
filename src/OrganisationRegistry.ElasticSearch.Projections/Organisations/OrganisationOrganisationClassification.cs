namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationClassificationType.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationClassification.Events;
    using Common;

    public class OrganisationOrganisationClassification :
        BaseProjection<OrganisationOrganisationClassification>,
        IEventHandler<OrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
        IEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminationSyncedWithKbo>,
        IEventHandler<OrganisationOrganisationClassificationUpdated>,
        IEventHandler<OrganisationClassificationTypeUpdated>,
        IEventHandler<OrganisationClassificationUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationOrganisationClassification(
            ILogger<OrganisationOrganisationClassification> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.OrganisationClassifications.Single().OrganisationClassificationTypeId, message.Body.OrganisationClassificationTypeId,
                    "organisationClassifications", "organisationClassificationTypeId",
                    "organisationClassificationTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.OrganisationClassifications.Single().OrganisationClassificationId, message.Body.OrganisationClassificationId,
                    "organisationClassifications", "organisationClassificationId",
                    "organisationClassificationName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.OrganisationOrganisationClassificationId,
                message.Body.OrganisationClassificationTypeId,
                message.Body.OrganisationClassificationTypeName,
                message.Body.OrganisationClassificationId,
                message.Body.OrganisationClassificationName,
                message.Body.ValidFrom,
                message.Body.ValidTo,
                message.Number,
                message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
        {
            AddOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.OrganisationOrganisationClassificationId,
                message.Body.OrganisationClassificationTypeId,
                message.Body.OrganisationClassificationTypeName,
                message.Body.OrganisationClassificationId,
                message.Body.OrganisationClassificationName,
                message.Body.ValidFrom,
                message.Body.ValidTo,
                message.Number,
                message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
        {
            RemoveOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.OrganisationOrganisationClassificationId,
                message.Number,
                message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel == null)
                return;

            RemoveOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel.Value,
                message.Number,
                message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate == null)
                return;

            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.OrganisationClassifications == null)
                organisationDocument.OrganisationClassifications = new List<OrganisationDocument.OrganisationOrganisationClassification>();

            var legalFormOrganisationClassification = organisationDocument.OrganisationClassifications.Single(label =>
                label.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate);

            legalFormOrganisationClassification.Validity.End = message.Body.DateOfTermination;

            _elastic.Try(async () => (await _elastic.WriteClient.IndexDocumentAsync(organisationDocument)).ThrowOnFailure());
        }

        private void AddOrganisationOrganisationClassification(Guid organisationId, Guid organisationOrganisationClassificationId, Guid organisationClassificationTypeId, string organisationClassificationTypeName, Guid organisationClassificationId, string organisationClassificationName, DateTime? validFrom, DateTime? validTo, int messageNumber, DateTimeOffset messageTimestamp)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = messageNumber;
            organisationDocument.ChangeTime = messageTimestamp;

            if (organisationDocument.OrganisationClassifications == null)
                organisationDocument.OrganisationClassifications =
                    new List<OrganisationDocument.OrganisationOrganisationClassification>();

            organisationDocument.OrganisationClassifications.RemoveExistingListItems(x =>
                x.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

            organisationDocument.OrganisationClassifications.Add(
                new OrganisationDocument.OrganisationOrganisationClassification(
                    organisationOrganisationClassificationId,
                    organisationClassificationTypeId,
                    organisationClassificationTypeName,
                    organisationClassificationId,
                    organisationClassificationName,
                    new Period(validFrom, validTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        private void RemoveOrganisationOrganisationClassification(Guid organisationId, Guid organisationOrganisationClassificationId, int messageNumber, DateTimeOffset messageTimestamp)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = messageNumber;
            organisationDocument.ChangeTime = messageTimestamp;

            if (organisationDocument.OrganisationClassifications == null)
                organisationDocument.OrganisationClassifications =
                    new List<OrganisationDocument.OrganisationOrganisationClassification>();

            organisationDocument.OrganisationClassifications.RemoveExistingListItems(x =>
                x.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.OrganisationClassifications.RemoveExistingListItems(x => x.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

            organisationDocument.OrganisationClassifications.Add(
                new OrganisationDocument.OrganisationOrganisationClassification(
                    message.Body.OrganisationOrganisationClassificationId,
                    message.Body.OrganisationClassificationTypeId,
                    message.Body.OrganisationClassificationTypeName,
                    message.Body.OrganisationClassificationId,
                    message.Body.OrganisationClassificationName,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
