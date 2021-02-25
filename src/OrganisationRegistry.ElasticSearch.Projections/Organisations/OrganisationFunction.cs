namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Function.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Person.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using Common;

    public class OrganisationFunction :
        BaseProjection<OrganisationFunction>,
        IEventHandler<OrganisationFunctionAdded>,
        IEventHandler<OrganisationFunctionUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<PersonUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;
        private readonly IMemoryCaches _memoryCaches;

        public OrganisationFunction(
            ILogger<OrganisationFunction> logger,
            Elastic elastic,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _elastic = elastic;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Functions.Single().FunctionId, message.Body.FunctionId,
                    "functions", "functionId",
                    "functionName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Functions.Single().PersonId, message.Body.PersonId,
                    "functions", "personId",
                    "personName", $"{message.Body.Name} {message.Body.FirstName}",
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Functions == null)
                organisationDocument.Functions = new List<OrganisationDocument.OrganisationFunction>();

            organisationDocument.Functions.RemoveExistingListItems(x => x.OrganisationFunctionId == message.Body.OrganisationFunctionId);

            organisationDocument.Functions.Add(
                new OrganisationDocument.OrganisationFunction(
                    message.Body.OrganisationFunctionId,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.PersonId,
                    message.Body.PersonFullName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Functions.RemoveExistingListItems(x => x.OrganisationFunctionId == message.Body.OrganisationFunctionId);

            organisationDocument.Functions.Add(
                new OrganisationDocument.OrganisationFunction(
                    message.Body.OrganisationFunctionId,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.PersonId,
                    message.Body.PersonFullName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            foreach (var (key, value) in message.Body.FieldsToTerminate.Functions)
            {
                var organisationFunction =
                    organisationDocument
                        .Functions
                        .Single(x => x.OrganisationFunctionId == key);

                organisationFunction.Validity.End = value;
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
