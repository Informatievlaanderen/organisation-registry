namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Common;
    using ElasticSearch.People;
    using Function.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;

    public class PersonFunction :
        BaseProjection<PersonFunction>,
        IEventHandler<OrganisationFunctionAdded>,
        IEventHandler<OrganisationFunctionUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>
    {
        private readonly Elastic _elastic;
        private readonly IMemoryCaches _memoryCaches;

        public PersonFunction(
            ILogger<PersonFunction> logger,
            Elastic elastic,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _elastic = elastic;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionAdded> message)
        {
            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Functions == null)
                personDocument.Functions = new List<PersonDocument.PersonFunction>();

            personDocument.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

            personDocument.Functions.Add(
                new PersonDocument.PersonFunction(
                    message.Body.OrganisationFunctionId,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.OrganisationId,
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionUpdated> message)
        {
            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                var previousPersonDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PreviousPersonId).ThrowOnFailure().Source);
                previousPersonDocument.ChangeId = message.Number;
                previousPersonDocument.ChangeTime = message.Timestamp;

                if (previousPersonDocument.Functions == null)
                    previousPersonDocument.Functions = new List<PersonDocument.PersonFunction>();

                previousPersonDocument.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousPersonDocument).ThrowOnFailure());
            }

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Functions == null)
                personDocument.Functions = new List<PersonDocument.PersonFunction>();

            personDocument.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

            personDocument.Functions.Add(
                new PersonDocument.PersonFunction(
                    message.Body.OrganisationFunctionId,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.OrganisationId,
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Functions.Single().FunctionId, message.Body.FunctionId,
                    "functions", "functionId",
                    "functionName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private void MassUpdateFunctionOrganisationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Functions.Single().OrganisationId, organisationId,
                    "functions", "organisationId",
                    "organisationName", name,
                    messageNumber,
                    timestamp));
        }
    }
}
