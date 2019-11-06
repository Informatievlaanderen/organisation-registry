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
    using LabelType.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationLabel :
        BaseProjection<OrganisationLabel>,
        IEventHandler<OrganisationLabelAdded>,
        IEventHandler<KboFormalNameLabelAdded>,
        IEventHandler<OrganisationLabelUpdated>,
        IEventHandler<LabelTypeUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationLabel(
            ILogger<OrganisationLabel> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Labels.Single().LabelTypeId, message.Body.LabelTypeId,
                    "labels", "labelTypeId",
                    "labelTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelAdded> message)
        {
            AddLabel(message.Body.OrganisationId, message.Body.OrganisationLabelId, message.Body.LabelTypeId, message.Body.LabelTypeName, message.Body.Value, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelAdded> message)
        {
            AddLabel(message.Body.OrganisationId, message.Body.OrganisationLabelId, message.Body.LabelTypeId, message.Body.LabelTypeName, message.Body.Value, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);
        }

        private void AddLabel(Guid organisationId, Guid organisationLabelId, Guid labelTypeId, string labelTypeName, string labelValue, DateTime? validFrom, DateTime? validTo, int documentChangeId, DateTimeOffset timestamp)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = documentChangeId;
            organisationDocument.ChangeTime = timestamp;

            if (organisationDocument.Labels == null)
                organisationDocument.Labels = new List<OrganisationDocument.OrganisationLabel>();

            organisationDocument.Labels.RemoveExistingListItems(x => x.OrganisationLabelId == organisationLabelId);

            organisationDocument.Labels.Add(
                new OrganisationDocument.OrganisationLabel(
                    organisationLabelId,
                    labelTypeId,
                    labelTypeName,
                    labelValue,
                    new Period(validFrom, validTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Labels.RemoveExistingListItems(x => x.OrganisationLabelId == message.Body.OrganisationLabelId);

            organisationDocument.Labels.Add(
                new OrganisationDocument.OrganisationLabel(
                    message.Body.OrganisationLabelId,
                    message.Body.LabelTypeId,
                    message.Body.LabelTypeName,
                    message.Body.Value,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
