namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
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

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
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

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
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

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
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

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
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

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
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
