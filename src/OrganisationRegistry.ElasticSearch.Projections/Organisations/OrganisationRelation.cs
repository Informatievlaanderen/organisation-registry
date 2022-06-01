namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Common;
using ElasticSearch.Organisations;
using Infrastructure;
using Infrastructure.Change;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRelationType.Events;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using SqlServer;

public class OrganisationRelation : BaseProjection<OrganisationRelation>,
    IElasticEventHandler<OrganisationRelationAdded>,
    IElasticEventHandler<OrganisationRelationUpdated>,
    IElasticEventHandler<OrganisationRelationTypeUpdated>,
    IElasticEventHandler<OrganisationInfoUpdated>,
    IElasticEventHandler<OrganisationNameUpdated>,
    IElasticEventHandler<OrganisationInfoUpdatedFromKbo>,
    IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
    IElasticEventHandler<OrganisationTerminated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{
    private readonly IContextFactory _contextFactory;

    public OrganisationRelation(
        ILogger<OrganisationRelation> logger,
        IContextFactory contextFactory) : base(logger)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationAdded> message)
    {
        var changes = new Dictionary<Guid, Func<OrganisationDocument, Task>>();
        //initiator
        changes.Add(message.Body.OrganisationId, async document =>
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var relatedOrganisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.RelatedOrganisationId);

            document.ChangeId = message.Number;
            document.ChangeTime = message.Timestamp;

            document.Relations.RemoveExistingListItems(x => x.OrganisationRelationId == message.Body.OrganisationRelationId);
            document.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationName,
                    message.Body.RelatedOrganisationId,
                    relatedOrganisation.OvoNumber,
                    relatedOrganisation.Name,
                    Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
        });
        //relation
        changes.Add(message.Body.RelatedOrganisationId, async document =>
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);

            document.ChangeId = message.Number;
            document.ChangeTime = message.Timestamp;

            document.Relations.RemoveExistingListItems(x => x.OrganisationRelationId == message.Body.OrganisationRelationId);
            document.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationInverseName,
                    message.Body.OrganisationId,
                    organisation.OvoNumber,
                    organisation.Name,
                    Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));

        });
        return await new ElasticPerDocumentChange<OrganisationDocument>(changes).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationUpdated> message)
    {
        var changes = new Dictionary<Guid, Func<OrganisationDocument, Task>>();

        //initiator
        changes.Add(message.Body.OrganisationId, async document =>
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var relatedOrganisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.RelatedOrganisationId);

            document.ChangeId = message.Number;
            document.ChangeTime = message.Timestamp;

            document.Relations.RemoveExistingListItems(x =>
                x.OrganisationRelationId == message.Body.OrganisationRelationId);
            document.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationName,
                    message.Body.RelatedOrganisationId,
                    relatedOrganisation.OvoNumber,
                    relatedOrganisation.Name,
                    Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));

        });

        //relation
        changes.Add(message.Body.RelatedOrganisationId, async document =>
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);

            document.ChangeId = message.Number;
            document.ChangeTime = message.Timestamp;

            document.Relations.RemoveExistingListItems(x =>
                x.OrganisationRelationId == message.Body.OrganisationRelationId);
            document.Relations.Add(
                new OrganisationDocument.OrganisationRelation(
                    message.Body.OrganisationRelationId,
                    message.Body.RelationId,
                    message.Body.RelationInverseName,
                    message.Body.OrganisationId,
                    organisation.OvoNumber,
                    organisation.Name,
                    Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
        });

        return await new ElasticPerDocumentChange<OrganisationDocument>(changes).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        => await MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
        => await MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        => await MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        => await MassUpdateOrganisationRelationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);

    private static async Task<IElasticChange> MassUpdateOrganisationRelationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        => await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Relations.Single().RelatedOrganisationId, organisationId,
                    "relations", "relationId",
                    "relationName", name,
                    messageNumber,
                    timestamp))
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeUpdated> message)
        => await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Relations.Single().RelationId, message.Body.OrganisationRelationTypeId,
                    "relations", "relationId",
                    "relationName", message.Body.Name,
                    message.Number,
                    message.Timestamp))
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        => await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                foreach (var (key, value) in message.Body.FieldsToTerminate.Relations)
                {
                    var organisationRelation =
                        document
                            .Relations
                            .Single(x => x.OrganisationRelationId == key);

                    organisationRelation.Validity.End = value;
                }
            }
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        => await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                foreach (var (key, value) in message.Body.FieldsToTerminate.Relations)
                {
                    var organisationRelation =
                        document
                            .Relations
                            .Single(x => x.OrganisationRelationId == key);

                    organisationRelation.Validity.End = value;
                }
            }
        ).ToAsyncResult();
}
