namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Common;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using OrganisationRelationType.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;

    public class OrganisationRelation : BaseProjection<OrganisationRelation>,
        IEventHandler<OrganisationRelationAdded>,
        IEventHandler<OrganisationRelationUpdated>,
        IEventHandler<OrganisationRelationTypeUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;
        private readonly IMemoryCaches _memoryCaches;

        public OrganisationRelation(
            ILogger<OrganisationRelation> logger,
            IMemoryCaches memoryCaches,
            Elastic elastic) : base(logger)
        {
            _memoryCaches = memoryCaches;
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationAdded> message)
        {
            //initiator
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Relations == null)
                organisationDocument.Relations = new List<OrganisationDocument.OrganisationRelation>();
            organisationDocument.Relations.RemoveExistingListItems(x => x.OrganisationRelationId == message.Body.OrganisationRelationId);
            organisationDocument.Relations.Add(
                    new OrganisationDocument.OrganisationRelation(
                        message.Body.OrganisationRelationId,
                        message.Body.RelationId,
                        message.Body.RelationName,
                        message.Body.RelatedOrganisationId,
                        _memoryCaches.OvoNumbers[message.Body.RelatedOrganisationId],
                        _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId],
                        new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());

            //relation
            var relatedOrganisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.RelatedOrganisationId).ThrowOnFailure().Source);

            relatedOrganisationDocument.ChangeId = message.Number;
            relatedOrganisationDocument.ChangeTime = message.Timestamp;

            if (relatedOrganisationDocument.Relations == null)
                relatedOrganisationDocument.Relations = new List<OrganisationDocument.OrganisationRelation>();
            relatedOrganisationDocument.Relations.RemoveExistingListItems(x => x.OrganisationRelationId == message.Body.OrganisationRelationId);
            relatedOrganisationDocument.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationInverseName,
                    message.Body.OrganisationId,
                    _memoryCaches.OvoNumbers[message.Body.OrganisationId],
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(relatedOrganisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationUpdated> message)
        {
            //initiator
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure()
                    .Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Relations == null)
                organisationDocument.Relations = new List<OrganisationDocument.OrganisationRelation>();
            organisationDocument.Relations.RemoveExistingListItems(x =>
                x.OrganisationRelationId == message.Body.OrganisationRelationId);
            organisationDocument.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationName,
                    message.Body.RelatedOrganisationId,
                    _memoryCaches.OvoNumbers[message.Body.RelatedOrganisationId],
                    _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId],
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());

            //relation
            var relatedOrganisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(message.Body.RelatedOrganisationId).ThrowOnFailure()
                    .Source);

            relatedOrganisationDocument.ChangeId = message.Number;
            relatedOrganisationDocument.ChangeTime = message.Timestamp;

            if (relatedOrganisationDocument.Relations == null)
                relatedOrganisationDocument.Relations = new List<OrganisationDocument.OrganisationRelation>();
            relatedOrganisationDocument.Relations.RemoveExistingListItems(x =>
                x.OrganisationRelationId == message.Body.OrganisationRelationId);
            relatedOrganisationDocument.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationInverseName,
                    message.Body.OrganisationId,
                    _memoryCaches.OvoNumbers[message.Body.OrganisationId],
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(relatedOrganisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private void MassUpdateOrganisationRelationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
// Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Relations.Single().RelatedOrganisationId, organisationId,
                    "relations", "relationId",
                    "relationName", name,
                    messageNumber,
                    timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Relations.Single().RelationId, message.Body.OrganisationRelationTypeId,
                    "relations", "relationId",
                    "relationName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            foreach (var (key, value) in message.Body.FieldsToTerminate.RelationsToTerminate)
            {
                var organisationRelation =
                    organisationDocument
                        .Relations
                        .Single(x => x.OrganisationRelationId == key);

                organisationRelation.Validity.End = value;
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
