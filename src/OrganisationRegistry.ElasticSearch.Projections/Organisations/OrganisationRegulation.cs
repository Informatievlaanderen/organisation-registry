namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Organisations;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Infrastructure.Events;
using RegulationTheme.Events;
using Infrastructure;
using Microsoft.Extensions.Logging;
using Common;
using Infrastructure.Change;
using RegulationSubTheme.Events;

public class OrganisationRegulation :
    BaseProjection<OrganisationRegulation>,
    IElasticEventHandler<OrganisationRegulationAdded>,
    IElasticEventHandler<OrganisationRegulationUpdated>,
    IElasticEventHandler<RegulationThemeUpdated>,
    IElasticEventHandler<RegulationSubThemeUpdated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{

    public OrganisationRegulation(
        ILogger<OrganisationRegulation> logger) : base(logger)
    {
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationThemeUpdated> message)
        => await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Regulations.Single().RegulationThemeId, message.Body.RegulationThemeId,
                    "regulations", "regulationThemeId",
                    "regulationThemeName", message.Body.Name,
                    message.Number,
                    message.Timestamp))
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RegulationSubThemeUpdated> message)
        => await new ElasticMassChange
        (
            elastic => elastic.TryAsync(() => elastic
                .MassUpdateOrganisationAsync(
                    x => x.Regulations.Single().RegulationSubThemeId, message.Body.RegulationSubThemeId,
                    "regulations", "regulationSubThemeId",
                    "regulationSubThemeName", message.Body.Name,
                    message.Number,
                    message.Timestamp))
        ).ToAsyncResult();

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRegulationAdded> message)
        => await AddOrganisationRegulation(
            message.Body.OrganisationId,
            message.Body.OrganisationRegulationId,
            message.Body.RegulationThemeId,
            message.Body.RegulationThemeName,
            message.Body.RegulationSubThemeId,
            message.Body.RegulationSubThemeName,
            message.Body.Name,
            message.Body.ValidFrom,
            message.Body.ValidTo,
            message.Body.DescriptionRendered,
            message.Body.Uri,
            message.Body.WorkRulesUrl,
            message.Body.Date,
            message.Number, message.Timestamp);

    private static async Task<IElasticChange> AddOrganisationRegulation(Guid organisationId,
        Guid organisationRegulationId,
        Guid? regulationThemeId,
        string? regulationThemeName,
        Guid? regulationSubThemeId,
        string? regulationSubThemeName,
        string name,
        DateTime? validFrom,
        DateTime? validTo,
        string? descriptionRendered,
        string? regulationUrl,
        string? workRulesUrl,
        DateTime? regulationDate,
        int messageNumber, DateTimeOffset messageTimestamp)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            organisationId,
            document =>
            {
                document.ChangeId = messageNumber;
                document.ChangeTime = messageTimestamp;

                document.Regulations.RemoveExistingListItems(
                    x =>
                        x.OrganisationRegulationId == organisationRegulationId);


                document.Regulations.Add(
                    new OrganisationDocument.OrganisationRegulation(
                        organisationRegulationId,
                        regulationThemeId,
                        regulationThemeName,
                        regulationSubThemeId,
                        regulationSubThemeName,
                        name,
                        description: descriptionRendered,
                        regulationUrl,
                        workRulesUrl,
                        regulationDate,
                        Period.FromDates(validFrom, validTo)));
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRegulationUpdated> message)
        => await new ElasticPerDocumentChange<OrganisationDocument>
        (
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Regulations.RemoveExistingListItems(x => x.OrganisationRegulationId == message.Body.OrganisationRegulationId);

                document.Regulations.Add(
                    new OrganisationDocument.OrganisationRegulation(
                        message.Body.OrganisationRegulationId,
                        message.Body.RegulationThemeId,
                        message.Body.RegulationThemeName,
                        message.Body.RegulationSubThemeId,
                        message.Body.RegulationSubThemeName,
                        message.Body.Name,
                        description: message.Body.DescriptionRendered,
                        message.Body.Url,
                        message.Body.WorkRulesUrl,
                        message.Body.Date,
                        Period.FromDates(message.Body.ValidFrom, message.Body.ValidTo)));
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

                foreach (var (key, value) in message.Body.FieldsToTerminate.Regulations)
                {
                    var organisationRegulation =
                        document
                            .Regulations
                            .Single(x => x.OrganisationRegulationId == key);

                    organisationRegulation.Validity.End = value;
                }
            }
        ).ToAsyncResult();
}
