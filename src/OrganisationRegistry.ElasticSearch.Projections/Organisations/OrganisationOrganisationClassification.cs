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
    using Infrastructure.Change;

    public class OrganisationOrganisationClassification :
        BaseProjection<OrganisationOrganisationClassification>,
        IElasticEventHandler<OrganisationOrganisationClassificationAdded>,
        IElasticEventHandler<KboLegalFormOrganisationOrganisationClassificationAdded>,
        IElasticEventHandler<KboLegalFormOrganisationOrganisationClassificationRemoved>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
        IElasticEventHandler<OrganisationOrganisationClassificationUpdated>,
        IElasticEventHandler<OrganisationClassificationTypeUpdated>,
        IElasticEventHandler<OrganisationClassificationUpdated>,
        IElasticEventHandler<OrganisationTerminated>
    {

        public OrganisationOrganisationClassification(
            ILogger<OrganisationOrganisationClassification> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationTypeUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.OrganisationClassifications.Single().OrganisationClassificationTypeId, message.Body.OrganisationClassificationTypeId,
                        "organisationClassifications", "organisationClassificationTypeId",
                        "organisationClassificationTypeName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationClassificationUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.OrganisationClassifications.Single().OrganisationClassificationId, message.Body.OrganisationClassificationId,
                        "organisationClassifications", "organisationClassificationId",
                        "organisationClassificationName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationAdded> message)
        {
            return await AddOrganisationOrganisationClassification(
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

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationAdded> message)
        {
            return await AddOrganisationOrganisationClassification(
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

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboLegalFormOrganisationOrganisationClassificationRemoved> message)
        {
            return await RemoveOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.OrganisationOrganisationClassificationId,
                message.Number,
                message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel == null)
                return new ElasticNoChange();

            return await RemoveOrganisationOrganisationClassification(
                message.Body.OrganisationId,
                message.Body.LegalFormOrganisationOrganisationClassificationIdToCancel.Value,
                message.Number,
                message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate == null)
                return new ElasticNoChange();

            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.OrganisationClassifications == null)
                        document.OrganisationClassifications = new List<OrganisationDocument.OrganisationOrganisationClassification>();

                    var legalFormOrganisationClassification = document.OrganisationClassifications.Single(label =>
                        label.OrganisationOrganisationClassificationId == message.Body.LegalFormOrganisationOrganisationClassificationIdToTerminate);

                    legalFormOrganisationClassification.Validity.End = message.Body.DateOfTermination;
                }
            );
        }

        private static async Task<IElasticChange> AddOrganisationOrganisationClassification(Guid organisationId, Guid organisationOrganisationClassificationId, Guid organisationClassificationTypeId, string organisationClassificationTypeName, Guid organisationClassificationId, string organisationClassificationName, DateTime? validFrom, DateTime? validTo, int messageNumber, DateTimeOffset messageTimestamp)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                organisationId, async document =>
                {
                    document.ChangeId = messageNumber;
                    document.ChangeTime = messageTimestamp;

                    if (document.OrganisationClassifications == null)
                        document.OrganisationClassifications =
                            new List<OrganisationDocument.OrganisationOrganisationClassification>();

                    document.OrganisationClassifications.RemoveExistingListItems(x =>
                        x.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

                    document.OrganisationClassifications.Add(
                        new OrganisationDocument.OrganisationOrganisationClassification(
                            organisationOrganisationClassificationId,
                            organisationClassificationTypeId,
                            organisationClassificationTypeName,
                            organisationClassificationId,
                            organisationClassificationName,
                            new Period(validFrom, validTo)));

                }
            );
        }

        private static async Task<IElasticChange> RemoveOrganisationOrganisationClassification(Guid organisationId, Guid organisationOrganisationClassificationId, int messageNumber, DateTimeOffset messageTimestamp)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                organisationId, async document =>
                {
                    document.ChangeId = messageNumber;
                    document.ChangeTime = messageTimestamp;

                    if (document.OrganisationClassifications == null)
                        document.OrganisationClassifications =
                            new List<OrganisationDocument.OrganisationOrganisationClassification>();

                    document.OrganisationClassifications.RemoveExistingListItems(x =>
                        x.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationOrganisationClassificationUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.OrganisationClassifications.RemoveExistingListItems(x => x.OrganisationOrganisationClassificationId == message.Body.OrganisationOrganisationClassificationId);

                    document.OrganisationClassifications.Add(
                        new OrganisationDocument.OrganisationOrganisationClassification(
                            message.Body.OrganisationOrganisationClassificationId,
                            message.Body.OrganisationClassificationTypeId,
                            message.Body.OrganisationClassificationTypeName,
                            message.Body.OrganisationClassificationId,
                            message.Body.OrganisationClassificationName,
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
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

                    var classificationsToTerminate =
                        message.Body.FieldsToTerminate.Classifications;

                    if (message.Body.KboFieldsToTerminate.LegalForm.HasValue)
                        classificationsToTerminate.Add(message.Body.KboFieldsToTerminate.LegalForm.Value.Key, message.Body.KboFieldsToTerminate.LegalForm.Value.Value);

                    foreach (var (key, value) in classificationsToTerminate)
                    {
                        var organisationOrganisationClassification =
                            document
                                .OrganisationClassifications
                                .Single(x => x.OrganisationOrganisationClassificationId == key);

                        organisationOrganisationClassification.Validity.End = value;
                    }
                }
            );
        }
    }
}
