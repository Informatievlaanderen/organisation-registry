namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Cache;
using Common;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Function.Events;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.Extensions.Logging;
using Organisation.Events;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer;

public class PersonFunction :
    BaseProjection<PersonFunction>,
    IElasticEventHandler<OrganisationFunctionAdded>,
    IElasticEventHandler<OrganisationFunctionUpdated>,
    IElasticEventHandler<OrganisationFunctionRemoved>,
    IElasticEventHandler<FunctionUpdated>,
    IElasticEventHandler<OrganisationInfoUpdated>,
    IElasticEventHandler<OrganisationNameUpdated>,
    IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
    IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
    IElasticEventHandler<OrganisationTerminated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{
    private readonly IPersonHandlerCache _cache;
    private readonly IContextFactory _contextFactory;

    public PersonFunction(
        ILogger<PersonFunction> logger,
        IContextFactory contextFactory,
        IPersonHandlerCache cache) : base(logger)
    {
        _contextFactory = contextFactory;
        _cache = cache;
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<FunctionUpdated> message)
        => await new ElasticMassChange
        (
            async elastic => await elastic
                .MassUpdatePersonAsync(
                    x => x.Functions.Single().FunctionId,
                    message.Body.FunctionId,
                    "functions",
                    "functionId",
                    "functionName",
                    message.Body.Name,
                    message.Number,
                    message.Timestamp)
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationCouplingWithKboCancelled> message)
        => await MassUpdateFunctionOrganisationName(
            message.Body.OrganisationId,
            message.Body.NameBeforeKboCoupling,
            message.Number,
            message.Timestamp).ToAsyncResult();

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationFunctionAdded> message)
        => await new ElasticPerDocumentChange<PersonDocument>(
            message.Body.PersonId,
            async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();
                var organisation = await _cache.GetOrganisation(
                    organisationRegistryContext,
                    message.Body.OrganisationId);
                var contactTypeNames = await _cache.GetContactTypeNames(organisationRegistryContext);

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Functions.RemoveExistingListItems(
                    x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                document.Functions.Add(
                    new PersonDocument.PersonFunction(
                        message.Body.OrganisationFunctionId,
                        message.Body.FunctionId,
                        message.Body.FunctionName,
                        message.Body.OrganisationId,
                        organisation.Name,
                        message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value))
                            .ToList(),
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            }
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationFunctionUpdated> message)
    {
        var changes = new Dictionary<Guid, Func<PersonDocument, Task>>();

        // If previous exists and current is different, we need to delete and add
        if (message.Body.PreviousPersonId != message.Body.PersonId)
            changes.Add(
                message.Body.PreviousPersonId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Functions.RemoveExistingListItems(
                        x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                    return Task.CompletedTask;
                });

        changes.Add(
            message.Body.PersonId,
            async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();
                var organisation = await _cache.GetOrganisation(
                    organisationRegistryContext,
                    message.Body.OrganisationId);
                var contactTypeNames = await _cache.GetContactTypeNames(organisationRegistryContext);

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Functions.RemoveExistingListItems(
                    x => x.PersonFunctionId == message.Body.OrganisationFunctionId);

                document.Functions.Add(
                    new PersonDocument.PersonFunction(
                        message.Body.OrganisationFunctionId,
                        message.Body.FunctionId,
                        message.Body.FunctionName,
                        message.Body.OrganisationId,
                        organisation.Name,
                        message.Body.Contacts.Select(x => new Contact(x.Key, contactTypeNames[x.Key], x.Value))
                            .ToList(),
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            });

        return await new ElasticPerDocumentChange<PersonDocument>(changes).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationFunctionRemoved> message)
    {
        var changes = new Dictionary<Guid, Func<PersonDocument, Task>>();

        changes.Add(
            message.Body.PersonId,
            async document =>
            {
                await using var organisationRegistryContext = _contextFactory.Create();

                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Functions.RemoveExistingListItems(
                    x => x.PersonFunctionId == message.Body.OrganisationFunctionId);
            });

        return await new ElasticPerDocumentChange<PersonDocument>(changes).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdated> message)
        => await MassUpdateFunctionOrganisationName(
            message.Body.OrganisationId,
            message.Body.Name,
            message.Number,
            message.Timestamp).ToAsyncResult();

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        => await MassUpdateFunctionOrganisationName(
            message.Body.OrganisationId,
            message.Body.Name,
            message.Number,
            message.Timestamp).ToAsyncResult();

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationNameUpdated> message)
        => await MassUpdateFunctionOrganisationName(
            message.Body.OrganisationId,
            message.Body.Name,
            message.Number,
            message.Timestamp).ToAsyncResult();

    private ElasticMassChange MassUpdateFunctionOrganisationName(
        Guid organisationId,
        string name,
        int messageNumber,
        DateTimeOffset timestamp)
        => new(
            async elastic => await elastic
                .MassUpdatePersonAsync(
                    x => x.Functions.Single().OrganisationId,
                    organisationId,
                    "functions",
                    "organisationId",
                    "organisationName",
                    name,
                    messageNumber,
                    timestamp)
        );

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        => await new ElasticMassChange(
            async elastic => await elastic
                .MassTerminatePersonFunctionsAsync(
                    x => x.Functions.Single().OrganisationId,
                    message.Body.OrganisationId,
                    message.Body.FieldsToTerminate.Functions,
                    message.Number,
                    message.Timestamp)
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        => await new ElasticMassChange(
            async elastic => await elastic
                .MassTerminatePersonFunctionsAsync(
                    x => x.Functions.Single().OrganisationId,
                    message.Body.OrganisationId,
                    message.Body.FieldsToTerminate.Functions,
                    message.Number,
                    message.Timestamp)
        ).ToAsyncResult();
}
