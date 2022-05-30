namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using System.Data.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Events;
using ElasticSearch.Organisations;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Common;
using Infrastructure.Change;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Body.Events;
using SqlServer;

public class OrganisationBody :
    BaseProjection<OrganisationBody>,
    IElasticEventHandler<BodyInfoChanged>,
    IElasticEventHandler<BodyOrganisationAdded>,
    IElasticEventHandler<BodyOrganisationUpdated>
{
    private readonly IContextFactory _contextFactory;

    public OrganisationBody(
        ILogger<OrganisationBody> logger,
        IContextFactory contextFactory) : base(logger)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
    {
        return await new ElasticMassChange
        (
            elastic => elastic.TryAsync(
                async () => await elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Bodies.Single().BodyId,
                        message.Body.BodyId,
                        "bodies",
                        "bodyId",
                        "bodyName",
                        message.Body.Name,
                        message.Number,
                        message.Timestamp))
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Bodies.RemoveExistingListItems(x => x.BodyOrganisationId == message.Body.BodyOrganisationId);

                document.Bodies.Add(
                    new OrganisationDocument.OrganisationBody(
                        message.Body.BodyOrganisationId,
                        message.Body.BodyId,
                        message.Body.BodyName,
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
    {
        var changes = new Dictionary<Guid, Func<OrganisationDocument, Task>>
        {
            {
                message.Body.PreviousOrganisationId, document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    var bodyName = document.Bodies.First(x => x.BodyOrganisationId == message.Body.BodyOrganisationId).BodyName;
                    document.Bodies.RemoveExistingListItems(x => x.BodyOrganisationId == message.Body.BodyOrganisationId);

                    if (message.Body.PreviousOrganisationId == message.Body.OrganisationId)
                    {
                        document.Bodies.Add(
                            new OrganisationDocument.OrganisationBody(
                                message.Body.BodyOrganisationId,
                                message.Body.BodyId,
                                bodyName,
                                Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                    }

                    return Task.CompletedTask;
                }
            }
        };

        if (message.Body.PreviousOrganisationId != message.Body.OrganisationId)
        {
            changes.Add(
                message.Body.OrganisationId,
                async document =>
                {
                    await using var organisationRegistryContext = _contextFactory.Create();
                    var body = await organisationRegistryContext.BodyCache.SingleAsync(x => x.Id == message.Body.BodyId);

                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    document.Bodies.Add(
                        new OrganisationDocument.OrganisationBody(
                            message.Body.BodyOrganisationId,
                            message.Body.BodyId,
                            body.Name,
                            Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
                });
        }

        return await new ElasticPerDocumentChange<OrganisationDocument>(changes).ToAsyncResult();
    }
}