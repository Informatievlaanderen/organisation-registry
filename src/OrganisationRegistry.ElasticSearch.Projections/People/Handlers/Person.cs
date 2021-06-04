namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Client;
    using Configuration;
    using ElasticSearch.People;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Person.Events;
    using SqlServer;

    public class Person :
        Infrastructure.BaseProjection<Person>,
        IElasticEventHandler<InitialiseProjection>,
        IElasticEventHandler<PersonCreated>,
        IElasticEventHandler<PersonUpdated>
    {
        private readonly Elastic _elastic;
        private readonly IContextFactory _contextFactory;
        private readonly ElasticSearchConfiguration _elasticSearchOptions;

        private string[] ProjectionTableNames =>
            new[]
            {
                ShowOnVlaamseOverheidSitesPerOrganisationListConfiguration.TableName,
                OrganisationPerBodyListConfiguration.TableName
            };

        public Person(
            ILogger<Person> logger,
            Elastic elastic,
            IContextFactory contextFactory,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
            _elasticSearchOptions = elasticSearchOptions.Value;
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<PersonCreated> message)
        {
            return new ElasticDocumentCreation<PersonDocument>
            (
                message.Body.PersonId,
                () => new PersonDocument
                {
                    ChangeId = message.Number,
                    ChangeTime = message.Timestamp,
                    Id = message.Body.PersonId,
                    FirstName = message.Body.FirstName,
                    Name = message.Body.Name
                });
        }


        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            return new ElasticPerDocumentChange<PersonDocument>
            (
                message.Body.PersonId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.FirstName = message.Body.FirstName;
                    document.Name = message.Body.Name;

                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(Person).FullName)
                return new ElasticNoChange();

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            await PrepareIndex(_elastic.WriteClient, true);

            await using var context = _contextFactory.Create();
            await context.Database.ExecuteSqlRawAsync(
                string.Concat(ProjectionTableNames.Select(tableName => $"DELETE FROM [ElasticSearchProjections].[{tableName}];")));

            return new ElasticNoChange();
        }

        private async Task PrepareIndex(IElasticClient client, bool deleteIndex)
        {
            var indexName = _elasticSearchOptions.PeopleWriteIndex;

            if (deleteIndex && (await client.DoesIndexExist(indexName)))
            {
                var deleteResult = await client.Indices.DeleteAsync(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete people index '{indexName}'.");
            }

            if (!await client.DoesIndexExist(indexName))
            {
                var indexResult = await client.Indices.CreateAsync(
                    indexName,
                    index => index
                        .Map<PersonDocument>(PersonDocument.Mapping)
                        .Settings(descriptor => descriptor
                            .NumberOfShards(_elasticSearchOptions.NumberOfShards)
                            .NumberOfReplicas(_elasticSearchOptions.NumberOfReplicas)));

                if (!indexResult.IsValid)
                    throw new Exception($"Could not create people index '{indexName}'.");
            }
        }
    }
}
