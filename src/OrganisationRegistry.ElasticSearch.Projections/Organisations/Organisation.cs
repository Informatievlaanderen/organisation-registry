namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Common;
    using Configuration;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation.Events;
    using Osc;
    using Purpose.Events;

    public class Organisation :
        BaseProjection<Organisation>,
        IElasticEventHandler<InitialiseProjection>,
        IElasticEventHandler<OrganisationCreated>,
        IElasticEventHandler<OrganisationCreatedFromKbo>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationNameUpdated>,
        IElasticEventHandler<OrganisationShortNameUpdated>,
        IElasticEventHandler<OrganisationArticleUpdated>,
        IElasticEventHandler<OrganisationValidityUpdated>,
        IElasticEventHandler<OrganisationOperationalValidityUpdated>,
        IElasticEventHandler<OrganisationDescriptionUpdated>,
        IElasticEventHandler<OrganisationShowOnVlaamseOverheidSitesUpdated>,
        IElasticEventHandler<OrganisationPurposesUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationCoupledWithKbo>,
        IElasticEventHandler<PurposeUpdated>,
        IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>,
        IElasticEventHandler<OrganisationPlacedUnderVlimpersManagement>,
        IElasticEventHandler<OrganisationReleasedFromVlimpersManagement>
    {
        private readonly Elastic _elastic;
        private readonly ElasticSearchConfiguration _elasticSearchOptions;
        private readonly IOrganisationManagementConfiguration _organisationManagementConfiguration;

        public Organisation(
            ILogger<Organisation> logger,
            Elastic elastic,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions,
            IOrganisationManagementConfiguration organisationManagementConfiguration) : base(logger)
        {
            _elastic = elastic;
            _elasticSearchOptions = elasticSearchOptions.Value;
            _organisationManagementConfiguration = organisationManagementConfiguration;
        }

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(Organisation).FullName)
                return new ElasticNoChange();

            Logger.LogInformation("Rebuilding index for {ProjectionName}", message.Body.ProjectionName);
            await PrepareIndex(_elastic.WriteClient, true);

            return new ElasticNoChange();
        }

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationArticleUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Article = message.Body.Article;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationCoupledWithKbo> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.KboNumber = message.Body.KboNumber;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationCouplingWithKboCancelled> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Name = message.Body.NameBeforeKboCoupling;
                    document.ShortName = message.Body.ShortNameBeforeKboCoupling;

                    document.KboNumber = string.Empty;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationCreated> message)
            => await new ElasticDocumentCreation<OrganisationDocument>(
                message.Body.OrganisationId,
                () => new OrganisationDocument
                {
                    ChangeId = message.Number,
                    ChangeTime = message.Timestamp,
                    Id = message.Body.OrganisationId,
                    Name = message.Body.Name,
                    OvoNumber = message.Body.OvoNumber,
                    ShortName = message.Body.ShortName,
                    Validity = Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo),
                    OperationalValidity =
                        Period.FromDates(message.Body.OperationalValidFrom, message.Body.OperationalValidTo),
                    Description = message.Body.Description,
                    ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                    Purposes = message.Body.Purposes
                        .Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList()
                }).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationCreatedFromKbo> message)
            => await new ElasticDocumentCreation<OrganisationDocument>(
                message.Body.OrganisationId,
                () => new OrganisationDocument
                {
                    ChangeId = message.Number,
                    ChangeTime = message.Timestamp,
                    Id = message.Body.OrganisationId,
                    Name = message.Body.Name,
                    OvoNumber = message.Body.OvoNumber,
                    ShortName = message.Body.ShortName,
                    Validity = Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo),
                    OperationalValidity =
                        Period.FromDates(message.Body.OperationalValidFrom, message.Body.OperationalValidTo),
                    Description = message.Body.Description,
                    KboNumber = message.Body.KboNumber,
                    ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites,
                    Purposes = message.Body.Purposes
                        .Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList()
                }).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationDescriptionUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Description = message.Body.Description;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationInfoUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                    document.Article = message.Body.Article;
                    document.Validity = Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo);
                    document.OperationalValidity = Period.FromDates(message.Body.OperationalValidFrom,
                        message.Body.OperationalValidTo);
                    document.Description = message.Body.Description;
                    document.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;
                    document.Purposes = message.Body.Purposes
                        .Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList();
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationInfoUpdatedFromKbo> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Name = message.Body.Name;
                    document.ShortName = message.Body.ShortName;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationNameUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Name = message.Body.Name;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationOperationalValidityUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.OperationalValidity = Period.FromDates(message.Body.OperationalValidFrom,
                        message.Body.OperationalValidTo);
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationPlacedUnderVlimpersManagement> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.ManagedBy = _organisationManagementConfiguration.Vlimpers;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationPurposesUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Purposes = message.Body.Purposes
                        .Select(x => new OrganisationDocument.Purpose(x.Id, x.Name)).ToList();
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationReleasedFromVlimpersManagement> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.ManagedBy = string.Empty;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationShortNameUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.ShortName = message.Body.ShortName;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationShowOnVlaamseOverheidSitesUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationTerminated> message)
            => message.Body.FieldsToTerminate.OrganisationValidity.HasValue
                ? await new ElasticPerDocumentChange<OrganisationDocument>(
                    message.Body.OrganisationId,
                    document =>
                    {
                        document.ChangeId = message.Number;
                        document.ChangeTime = message.Timestamp;

                        document.Validity =
                            Period.FromDates(document.Validity.Start, message.Body.FieldsToTerminate.OrganisationValidity);

                        document.OperationalValidity =
                            Period.FromDates(document.OperationalValidity?.Start,
                                message.Body.FieldsToTerminate.OrganisationValidity);

                        if (message.Body.ForcedKboTermination)
                            document.KboNumber = string.Empty;
                    }
                ).ToAsyncResult()
                : new ElasticNoChange();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationTerminatedV2> message)
            => message.Body.FieldsToTerminate.OrganisationValidity.HasValue
                ? await new ElasticPerDocumentChange<OrganisationDocument>(
                    message.Body.OrganisationId,
                    document =>
                    {
                        document.ChangeId = message.Number;
                        document.ChangeTime = message.Timestamp;

                        document.Validity =
                            Period.FromDates(document.Validity.Start, message.Body.FieldsToTerminate.OrganisationValidity);

                        document.OperationalValidity =
                            Period.FromDates(document.OperationalValidity?.Start,
                                message.Body.FieldsToTerminate.OrganisationValidity);

                        if (message.Body.ForcedKboTermination)
                            document.KboNumber = string.Empty;
                    }
                ).ToAsyncResult()
                : new ElasticNoChange();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationTerminationSyncedWithKbo> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.KboNumber = string.Empty;
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<OrganisationValidityUpdated> message)
            => await new ElasticPerDocumentChange<OrganisationDocument>(
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Validity = Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo);
                }
            ).ToAsyncResult();

        public async Task<IElasticChange> Handle(
            DbConnection dbConnection,
            DbTransaction dbTransaction,
            IEnvelope<PurposeUpdated> message)
            => await new ElasticMassChange
            (
                elastic => elastic.TryAsync(
                    () => elastic
                        .MassUpdateOrganisationAsync(
                            x => x.Purposes.Single().PurposeId,
                            message.Body.PurposeId,
                            "purposes",
                            "purposeId",
                            "purposeName",
                            message.Body.Name,
                            message.Number,
                            message.Timestamp))
            ).ToAsyncResult();

        private async Task PrepareIndex(IOpenSearchClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.OrganisationsWriteIndex;

            if (deleteIndex && await client.DoesIndexExist(indexName))
            {
                var deleteResult = await client.Indices.DeleteAsync(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete organisation index '{indexName}'.");
            }

            if (!await client.DoesIndexExist(indexName))
            {
                var indexResult = await client.Indices.CreateAsync(
                    indexName,
                    index => index
                        .Map<OrganisationDocument>(OrganisationDocument.Mapping)
                        .Settings(
                            descriptor => descriptor
                                .NumberOfShards(_elasticSearchOptions.NumberOfShards)
                                .NumberOfReplicas(_elasticSearchOptions.NumberOfReplicas)));

                if (!indexResult.IsValid)
                    throw new Exception($"Could not create organisation index '{indexName}'.");
            }
        }
    }
}
