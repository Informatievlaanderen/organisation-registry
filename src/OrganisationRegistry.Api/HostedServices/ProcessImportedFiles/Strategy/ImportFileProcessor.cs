namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;

public static class ImportFileProcessor
{
    public static async Task ProcessNextFile(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
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

        try
        {
            var processorFactory = new ImportedFileProcessorFactory(context, dateTimeProvider, commandSender);
            var result = await processorFactory
                .Create(importFile.ImportFileType)
                .Process(importFile, cancellationToken);

            UpdateImportFile(dateTimeProvider, logger, result.StatusItem, result.OutputFileContent, result.Success);
        }
        catch (Exception e)
        {
            logger.LogError(e, "An error occured while Importing files");
            UpdateImportFile(dateTimeProvider, logger, importFile, e.Message, false);
        }
        finally
        {
            await context.SaveChangesAsync(cancellationToken);
        }
    }

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

    private static async Task<ImportOrganisationsStatusListItem?> MaybeGetNextImportFile(
        OrganisationRegistryContext context,
        CancellationToken cancellationToken)
        => await context.ImportOrganisationsStatusList
            .Where(listItem => listItem.Status == ImportProcessStatus.Processing)
            .OrderBy(listItem => listItem.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);
}
