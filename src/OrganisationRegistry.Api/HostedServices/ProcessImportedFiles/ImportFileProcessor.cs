namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organisation;
using Organisation.Import;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;
using SqlServer.Import.Organisations;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public static class ImportFileProcessor
{
    public static async Task ProcessNextFile(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        IImportFileParserAndValidator importFileParserAndValidator,
        ICommandSender commandSender,
        HostedServiceConfiguration configuration,
        CancellationToken cancellationToken)
    {
        var context = contextFactory.Create();

        if (await MaybeGetNextImportFile(context, cancellationToken) is not { } importFile)
        {
            await configuration.Delay(cancellationToken);
            return;
        }

        var parseAndValidatorResult =
            importFileParserAndValidator.ParseAndValidate(importFile, context, dateTimeProvider);

        if (parseAndValidatorResult.ValidationOk)
        {
            var user = new User(importFile.UserFirstName, importFile.UserName, importFile.UserId, null, new[] { Role.AlgemeenBeheerder }, new List<string>());

            await commandSender.Send(new CreateOrganisationsFromImport(importFile.Id, parseAndValidatorResult.OutputRecords), user);

            var organisationDetails =
                await RetrieveOrganisationDetails(context, importFile.Id, parseAndValidatorResult.OutputRecords.Count, cancellationToken);

            var result = ToCsvResult(organisationDetails, parseAndValidatorResult.OutputRecords);

            UpdateImportFile(
                dateTimeProvider,
                logger,
                importFile,
                await OutputSerializer.Serialize(importFile, result),
                parseAndValidatorResult.ValidationOk);
        }
        else
        {
            UpdateImportFile(
                dateTimeProvider,
                logger,
                importFile,
                OutputSerializer.Serialize(parseAndValidatorResult.ValidationIssues),
                parseAndValidatorResult.ValidationOk);
        }

        // 3) resultaat van verwerking terug saven naar db
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<OutputRecord> ToCsvResult(
        IEnumerable<OrganisationDetailItem> organisationDetails,
        IEnumerable<OutputRecord> parsedRecords)
        => JoinRecordsWithProjection(organisationDetails, parsedRecords)
            .OrderBy(or => or.SortOrder)
            .ToList();

    private static IEnumerable<OutputRecord> JoinRecordsWithProjection(
        IEnumerable<OrganisationDetailItem> organisationDetails,
        IEnumerable<OutputRecord> parsedRecords)
        => from od in organisationDetails
            join outputRecord in parsedRecords on od.SourceOrganisationIdentifier equals outputRecord.Reference
            select outputRecord.WithOvoNumber(od.OvoNumber);

    /// <summary>
    /// Wait for the projection to be populated with the created organisations.
    /// </summary>
    /// <returns></returns>
    private static async Task<IEnumerable<OrganisationDetailItem>> RetrieveOrganisationDetails(
        OrganisationRegistryContext context,
        Guid importFileId,
        int importedNumberOfRecords,
        CancellationToken cancellationToken)
    {
        var details = new List<OrganisationDetailItem>();
        while (importedNumberOfRecords != details.Count)
        {
            details = await context.OrganisationDetail
                .Where(od => od.SourceType == OrganisationSource.CsvImport && od.SourceId == importFileId)
                .ToListAsync(cancellationToken);
            await Task.Delay(millisecondsDelay: 100, cancellationToken);
        }

        return details;
    }

    private static async Task<ImportOrganisationsStatusListItem?> MaybeGetNextImportFile(
        OrganisationRegistryContext context,
        CancellationToken cancellationToken)
        => await context.ImportOrganisationsStatusList
            .Where(listItem => listItem.Status == ImportProcessStatus.Processing)
            .OrderBy(listItem => listItem.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);

    private static void UpdateImportFile(
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        ImportOrganisationsStatusListItem importFile,
        string outputFileContent,
        bool success)
    {
        if (!success)
            logger.LogError("An error occured while processing the imported file {FileName}", importFile.FileName);

        importFile.Status = success ? ImportProcessStatus.Processed : ImportProcessStatus.Failed;
        importFile.LastProcessedAt = dateTimeProvider.UtcNow;
        importFile.OutputFileContent = outputFileContent;
    }
}
