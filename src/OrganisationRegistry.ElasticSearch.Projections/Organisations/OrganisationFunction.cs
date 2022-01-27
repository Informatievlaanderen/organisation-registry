namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Function.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Person.Events;
    using Common;
    using Infrastructure.Change;
    using Microsoft.EntityFrameworkCore;
    using SqlServer;

    public class OrganisationFunction :
        BaseProjection<OrganisationFunction>,
        IElasticEventHandler<OrganisationFunctionAdded>,
        IElasticEventHandler<OrganisationFunctionUpdated>,
        IElasticEventHandler<FunctionUpdated>,
        IElasticEventHandler<PersonUpdated>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {
        private readonly IContextFactory _contextFactory;

        public OrganisationFunction(
            ILogger<OrganisationFunction> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Functions.Single().FunctionId, message.Body.FunctionId,
                        "functions", "functionId",
                        "functionName", message.Body.Name,
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonUpdated> message)
        {
            return new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Functions.Single().PersonId, message.Body.PersonId,
                        "functions", "personId",
                        "personName", $"{message.Body.Name} {message.Body.FirstName}",
                        message.Number,
                        message.Timestamp))
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionAdded> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    if (document.Functions == null)
                        document.Functions = new List<OrganisationDocument.OrganisationFunction>();

                    document.Functions.RemoveExistingListItems(x => x.OrganisationFunctionId == message.Body.OrganisationFunctionId);

                    document.Functions.Add(
                        new OrganisationDocument.OrganisationFunction(
                            message.Body.OrganisationFunctionId,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFunctionUpdated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var contactTypeNames = await organisationRegistryContext.ContactTypeCache
                        .Select(x => new {x.Id, x.Name})
                        .ToDictionaryAsync(x => x.Id, x => x.Name);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Functions.RemoveExistingListItems(x => x.OrganisationFunctionId == message.Body.OrganisationFunctionId);

                    document.Functions.Add(
                        new OrganisationDocument.OrganisationFunction(
                            message.Body.OrganisationFunctionId,
                            message.Body.FunctionId,
                            message.Body.FunctionName,
                            message.Body.PersonId,
                            message.Body.PersonFullName,
                            message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value)).ToList(),
                            new Period(message.Body.ValidFrom, message.Body.ValidTo)));
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Functions)
                    {
                        var organisationFunction =
                            document
                                .Functions
                                .Single(x => x.OrganisationFunctionId == key);

                        organisationFunction.Validity.End = value;
                    }
                }
            );
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            return new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId, async document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    foreach (var (key, value) in message.Body.FieldsToTerminate.Functions)
                    {
                        var organisationFunction =
                            document
                                .Functions
                                .Single(x => x.OrganisationFunctionId == key);

                        organisationFunction.Validity.End = value;
                    }
                }
            );
        }
    }
}
