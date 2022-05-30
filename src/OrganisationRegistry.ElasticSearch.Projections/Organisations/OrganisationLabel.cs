namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Organisations;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Infrastructure.Events;
using LabelType.Events;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Common;
using Infrastructure.Change;

public class OrganisationLabel :
    BaseProjection<OrganisationLabel>,
    IElasticEventHandler<OrganisationLabelAdded>,
    IElasticEventHandler<KboFormalNameLabelAdded>,
    IElasticEventHandler<KboFormalNameLabelRemoved>,
    IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
    IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
    IElasticEventHandler<OrganisationLabelUpdated>,
    IElasticEventHandler<LabelTypeUpdated>,
    IElasticEventHandler<OrganisationTerminated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{
    public OrganisationLabel(
        ILogger<OrganisationLabel> logger) : base(logger)
    {
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LabelTypeUpdated> message)
    {
        return await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Labels.Single().LabelTypeId, message.Body.LabelTypeId,
                    "labels", "labelTypeId",
                    "labelTypeName", message.Body.Name,
                    message.Number,
                    message.Timestamp))
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelAdded> message)
        => await AddLabel(message.Body.OrganisationId, message.Body.OrganisationLabelId, message.Body.LabelTypeId, message.Body.LabelTypeName, message.Body.Value, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelAdded> message)
        => await AddLabel(message.Body.OrganisationId, message.Body.OrganisationLabelId, message.Body.LabelTypeId, message.Body.LabelTypeName, message.Body.Value, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboFormalNameLabelRemoved> message)
        => await RemoveLabel(message.Body.OrganisationId, message.Body.OrganisationLabelId, message.Number, message.Timestamp);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        if (message.Body.FormalNameOrganisationLabelIdToCancel == null)
            return new ElasticNoChange();

        return await RemoveLabel(message.Body.OrganisationId, message.Body.FormalNameOrganisationLabelIdToCancel.Value, message.Number, message.Timestamp);
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
    {
        if (message.Body.FormalNameOrganisationLabelIdToTerminate == null)
            return await new ElasticNoChange().ToAsyncResult();

        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var formalNameLabel = document.Labels.Single(label =>
                    label.OrganisationLabelId == message.Body.FormalNameOrganisationLabelIdToTerminate);

                formalNameLabel.Validity.End = message.Body.DateOfTermination;
            }
        ).ToAsyncResult();
    }

    private async Task<IElasticChange> AddLabel(Guid organisationId, Guid organisationLabelId, Guid labelTypeId, string labelTypeName, string labelValue, DateTime? validFrom, DateTime? validTo, int documentChangeId, DateTimeOffset timestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            organisationId,
            document =>
            {
                document.ChangeId = documentChangeId;
                document.ChangeTime = timestamp;

                document.Labels.RemoveExistingListItems(x => x.OrganisationLabelId == organisationLabelId);

                document.Labels.Add(
                    new OrganisationDocument.OrganisationLabel(
                        organisationLabelId,
                        labelTypeId,
                        labelTypeName,
                        labelValue,
                        Period.FromDates(validFrom, validTo)));
            }
        ).ToAsyncResult();
    }

    private async Task<IElasticChange> RemoveLabel(Guid organisationId, Guid organisationLabelId, int documentChangeId, DateTimeOffset timestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            organisationId,
            document =>
            {
                document.ChangeId = documentChangeId;
                document.ChangeTime = timestamp;

                document.Labels.RemoveExistingListItems(x => x.OrganisationLabelId == organisationLabelId);
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLabelUpdated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Labels.RemoveExistingListItems(x => x.OrganisationLabelId == message.Body.OrganisationLabelId);

                document.Labels.Add(
                    new OrganisationDocument.OrganisationLabel(
                        message.Body.OrganisationLabelId,
                        message.Body.LabelTypeId,
                        message.Body.LabelTypeName,
                        message.Body.Value,
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var labelsToTerminate =
                    message.Body.FieldsToTerminate.Labels;

                if (message.Body.KboFieldsToTerminate.FormalName.HasValue)
                    labelsToTerminate.Add(message.Body.KboFieldsToTerminate.FormalName.Value.Key, message.Body.KboFieldsToTerminate.FormalName.Value.Value);

                foreach (var (key, value) in labelsToTerminate)
                {
                    var organisationBankAccount =
                        document
                            .Labels
                            .Single(x => x.OrganisationLabelId == key);

                    organisationBankAccount.Validity.End = value;
                }
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var labelsToTerminate =
                    message.Body.FieldsToTerminate.Labels;

                if (message.Body.KboFieldsToTerminate.FormalName.HasValue)
                    labelsToTerminate.Add(message.Body.KboFieldsToTerminate.FormalName.Value.Key, message.Body.KboFieldsToTerminate.FormalName.Value.Value);

                foreach (var (key, value) in labelsToTerminate)
                {
                    var organisationBankAccount =
                        document
                            .Labels
                            .Single(x => x.OrganisationLabelId == key);

                    organisationBankAccount.Validity.End = value;
                }
            }
        ).ToAsyncResult();
    }
}