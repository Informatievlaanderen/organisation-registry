namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Capacity.Events;
    using Client;
    using ElasticSearch.Organisations;
    using Function.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Person.Events;
    using Common;
    using Microsoft.EntityFrameworkCore;
    using SqlServer;

    public class OrganisationCapacity :
        BaseProjection<OrganisationCapacity>,
        IEventHandler<OrganisationCapacityAdded>,
        IEventHandler<OrganisationCapacityUpdated>,
        IEventHandler<CapacityUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<PersonUpdated>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;
        private readonly IContextFactory _contextFactory;

        public OrganisationCapacity(
            ILogger<OrganisationCapacity> logger,
            Elastic elastic,
            IContextFactory contextFactory) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Capacities.Single().CapacityId, message.Body.CapacityId,
                    "capacities", "capacityId",
                    "capacityName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Capacities.Single().FunctionId, message.Body.FunctionId,
                    "capacities", "functionId",
                    "functionName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Capacities.Single().PersonId, message.Body.PersonId,
                    "capacities", "personId",
                    "personName", $"{message.Body.Name} {message.Body.FirstName}",
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeList
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);


            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Capacities == null)
                organisationDocument.Capacities = new List<OrganisationDocument.OrganisationCapacity>();

            organisationDocument.Capacities.RemoveExistingListItems(x => x.OrganisationCapacityId == message.Body.OrganisationCapacityId);

            organisationDocument.Capacities.Add(
                new OrganisationDocument.OrganisationCapacity(
                    message.Body.OrganisationCapacityId,
                    message.Body.CapacityId,
                    message.Body.CapacityName,
                    message.Body.PersonId,
                    message.Body.PersonFullName,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var contactTypeNames = await organisationRegistryContext.ContactTypeList
                .Select(x => new {x.Id, x.Name})
                .ToDictionaryAsync(x => x.Id, x => x.Name);


            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Capacities.RemoveExistingListItems(x => x.OrganisationCapacityId == message.Body.OrganisationCapacityId);

            organisationDocument.Capacities.Add(
                new OrganisationDocument.OrganisationCapacity(
                    message.Body.OrganisationCapacityId,
                    message.Body.CapacityId,
                    message.Body.CapacityName,
                    message.Body.PersonId,
                    message.Body.PersonFullName,
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            var organisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            foreach (var (key, value) in message.Body.FieldsToTerminate.Capacities)
            {
                var organisationCapacity =
                    organisationDocument
                        .Capacities
                        .Single(x => x.OrganisationCapacityId == key);

                organisationCapacity.Validity.End = value;
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
