namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Common;
    using ElasticSearch.People;
    using Function.Events;
    using Infrastructure;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class PersonFunction :
        BaseProjection<PersonFunction>,
        IElasticEventHandler<OrganisationFunctionAdded>,
        IElasticEventHandler<OrganisationFunctionUpdated>,
        IElasticEventHandler<FunctionUpdated>,
        IElasticEventHandler<OrganisationInfoUpdated>,
        IElasticEventHandler<OrganisationNameUpdated>,
        IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>
    {
        private readonly IContextFactory _contextFactory;

        public PersonFunction(
            ILogger<PersonFunction> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionAdded> message)
        {
            return new ElasticPerDocumentChange<PersonDocument>
            (
                message.Body.PersonId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);
                    var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Functions == null)
                        document.Functions = new List<PersonDocument.PersonFunction>();

                    document.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                    document.Functions.Add(
                        new PersonDocument.PersonFunction(
                            message.Body.OrganisationFunctionId,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.OrganisationId,
                            organisation.Name,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionUpdated> message)
        {
            var changes = new Dictionary<Guid, Action<PersonDocument>>();

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                changes.Add(message.Body.PreviousPersonId, document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Functions == null)
                        document.Functions = new List<PersonDocument.PersonFunction>();

                    document.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                });
            }

            changes.Add(message.Body.PersonId, async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();
                var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);
                var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                    .Select(x => new {x.Id, x.Name})
                    .ToDictionaryAsync(x => x.Id, x => x.Name);

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                if (document.Functions == null)
                    document.Functions = new List<PersonDocument.PersonFunction>();

                document.Functions.RemoveExistingListItems(x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                document.Functions.Add(
                    new PersonDocument.PersonFunction(
                        message.Body.OrganisationFunctionId,
                        message.Body.FunctionId,
                        message.Body.FunctionName,
                        message.Body.OrganisationId,
                        organisation.Name,
                        message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            });

            return new ElasticPerDocumentChange<PersonDocument>(changes);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            return new ElasticMassChange
            (
                async elastic => await elastic
                    .MassUpdatePersonAsync(
                        x => x.Functions.Single().FunctionId, message.Body.FunctionId,
                        "functions", "functionId",
                        "functionName", message.Body.Name,
                        message.Number,
                        message.Timestamp)
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            return MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        {
            return MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            return MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            return MassUpdateFunctionOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private ElasticMassChange MassUpdateFunctionOrganisationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            return new ElasticMassChange
            (
                async elastic => await elastic
                    .MassUpdatePersonAsync(
                        x => x.Functions.Single().OrganisationId, organisationId,
                        "functions", "organisationId",
                        "organisationName", name,
                        messageNumber,
                        timestamp)
            );
        }
    }
}
