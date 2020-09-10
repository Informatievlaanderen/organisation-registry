namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Data.Common;
    using System.Threading.Tasks;
    using Client;
    using Configuration;
    using Common;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Purpose.Events;

    public class Organisation :
        BaseProjection<Organisation>,
        IEventHandler<InitialiseProjection>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationCoupledWithKbo>,
        IEventHandler<PurposeUpdated>
    {
        private readonly Elastic _elastic;
        private readonly ElasticSearchConfiguration _elasticSearchOptions;

        public Organisation(
            ILogger<Organisation> logger,
            Elastic elastic,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions) : base(logger)
        {
            _elastic = elastic;
            _elasticSearchOptions = elasticSearchOptions.Value;

            PrepareIndex(elastic.WriteClient, false);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            var organisationDocument = new OrganisationDocument
            {
                ChangeId = message.Number,
                ChangeTime = message.Timestamp,
                Id = message.Body.OrganisationId,
                Name = message.Body.Name,
                OvoNumber = message.Body.OvoNumber,
                ShortName = message.Body.ShortName,
                Validity = new Period(message.Body.ValidFrom, message.Body.ValidTo),
                Description = message.Body.Description,
                ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                Purposes = message.Body.Purposes.Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList(),
            };

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            var organisationDocument = new OrganisationDocument
            {
                ChangeId = message.Number,
                ChangeTime = message.Timestamp,
                Id = message.Body.OrganisationId,
                Name = message.Body.Name,
                OvoNumber = message.Body.OvoNumber,
                ShortName = message.Body.ShortName,
                Validity = new Period(message.Body.ValidFrom, message.Body.ValidTo),
                Description = message.Body.Description,
                KboNumber = message.Body.KboNumber,
                ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                Purposes = message.Body.Purposes.Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList(),
            };

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Name = message.Body.Name;
            organisationDocument.ShortName = message.Body.ShortName;
            organisationDocument.Validity = new Period(message.Body.ValidFrom, message.Body.ValidTo);
            organisationDocument.Description = message.Body.Description;
            organisationDocument.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;
            organisationDocument.Purposes = message.Body.Purposes.Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList();

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Name = message.Body.Name;
            organisationDocument.ShortName = message.Body.ShortName;

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Name = message.Body.NameBeforeKboCoupling;
            organisationDocument.ShortName = message.Body.ShortNameBeforeKboCoupling;

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCoupledWithKbo> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.KboNumber = message.Body.KboNumber;

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PurposeUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Purposes.Single().PurposeId, message.Body.PurposeId,
                    "purposes", "purposeId",
                    "purposeName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(Organisation).FullName)
                return;

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            PrepareIndex(_elastic.WriteClient, true);
        }

        private void PrepareIndex(IElasticClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.OrganisationsWriteIndex;

            if (deleteIndex && client.DoesIndexExist(indexName))
            {
                var deleteResult = client.Indices.Delete(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete organisation index '{indexName}'.");
            }

            if (!client.DoesIndexExist(indexName))
            {
                var indexResult = client.Indices.Create(
                    indexName,
                    index => index
                        .Map<OrganisationDocument>(OrganisationDocument.Mapping)
                        .Settings(descriptor => descriptor
                            .NumberOfShards(_elasticSearchOptions.NumberOfShards)
                            .NumberOfReplicas(_elasticSearchOptions.NumberOfReplicas)));

                if (!indexResult.IsValid)
                    throw new Exception($"Could not create organisation index '{indexName}'.");
            }
        }
    }
}
