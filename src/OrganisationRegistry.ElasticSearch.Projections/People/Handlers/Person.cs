namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Client;
    using Configuration;
    using ElasticSearch.People;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Person.Events;

    public class Person :
        Infrastructure.BaseProjection<Person>,
        IEventHandler<InitialiseProjection>,
        IEventHandler<PersonCreated>,
        IEventHandler<PersonUpdated>
    {
        private readonly Elastic _elastic;
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly string _peopleIndex;
        private readonly string _personType;

        private string[] ProjectionTableNames =>
            new[]
            {
                ShowOnVlaamseOverheidSitesPerOrganisationListConfiguration.TableName,
                OrganisationPerBodyListConfiguration.TableName
            };

        public Person(
            ILogger<Person> logger,
            Elastic elastic,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
            _peopleIndex = elasticSearchOptions.Value.PeopleWriteIndex;
            _personType = elasticSearchOptions.Value.PersonType;

            PrepareIndex(elastic.WriteClient, false);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonCreated> message)
        {
            var personDocument = new PersonDocument
            {
                ChangeId = message.Number,
                ChangeTime = message.Timestamp,
                Id = message.Body.PersonId,
                FirstName = message.Body.FirstName,
                Name = message.Body.Name
            };

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            personDocument.FirstName = message.Body.FirstName;
            personDocument.Name = message.Body.Name;

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(Person).FullName)
                return;

            Logger.LogInformation("Rebuilding index for {ProjectionName}.", message.Body.ProjectionName);
            PrepareIndex(_elastic.WriteClient, true);

            using (var context = _contextFactory().Value)
                context.Database.ExecuteSqlCommand(
                    string.Concat(ProjectionTableNames.Select(tableName => $"DELETE FROM [OrganisationRegistry].[{tableName}];")));
        }

        private void PrepareIndex(IElasticClient client, bool deleteIndex)
        {
            var indexName = _peopleIndex;
            var typeName = _personType;

            if (deleteIndex && client.DoesIndexExist(indexName))
            {
                var deleteResult = client.Indices.Delete(
                    new DeleteIndexRequest(Indices.Index(new List<IndexName> { indexName })));

                if (!deleteResult.IsValid)
                    throw new Exception($"Could not delete people index '{indexName}'.");
            }

            if (!client.DoesIndexExist(indexName))
            {
                var indexResult = client.Indices.Create(
                    indexName,
                    index => index.Map<PersonDocument>(PersonDocument.Mapping));

                if (!indexResult.IsValid)
                    throw new Exception($"Could not create people index '{indexName}'.");
            }
        }
    }
}
