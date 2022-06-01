namespace OrganisationRegistry.Api.HostedServices;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProcessImportedFiles;
using SqlServer;
using SqlServer.Import.Organisations;
using SqlServer.Infrastructure;

public class ProcessImportedFilesService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProcessImportedFilesService> _logger;

    public ProcessImportedFilesService(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger<ProcessImportedFilesService> logger) : base(logger)
    {
        _contextFactory = contextFactory;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessNextFile(_contextFactory, _dateTimeProvider, _logger, cancellationToken);
        }
    }

    private static async Task ProcessNextFile(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var context = contextFactory.Create();

        if (await MaybeGetNextImportFile(context, cancellationToken) is not { } importFile)
        {
            await Task.Delay(15000, cancellationToken); // todo: move to config
            return;
        }

        var (validationOk, serializedOutput, csvOutput) = ImportFileParser.Parse(importFile);
        if (validationOk)
        {
            // TODO Process Records
        }

        UpdateImportFile(dateTimeProvider, logger, importFile, serializedOutput, validationOk);

        // 3) resultaat van verwerking terug saven naar db
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<ImportOrganisationsStatusListItem?> MaybeGetNextImportFile(OrganisationRegistryContext context, CancellationToken cancellationToken)
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
