namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProcessImportedFiles;
using SqlServer;
using SqlServer.Import.Organisations;

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
            await Task.Delay(15000, cancellationToken); // todo: move to config
        }
    }

    private static async Task ProcessNextFile(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        var context = contextFactory.Create();

        var maybeImportFile = await context.ImportOrganisationsStatusList
            .Where(listItem => listItem.Status == ImportProcessStatus.Processing)
            .OrderBy(listItem => listItem.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (maybeImportFile is not { } importFile)
            return; // Task.Wait?

        var (validationOk, serializedOutput, csvOutput) = ParseImportFile(importFile);
        if (validationOk)
        {
            // TODO Process Records
        }

        UpdateImportFile(dateTimeProvider, logger, importFile, serializedOutput, validationOk);

        // 3) resultaat van verwerking terug saven naar db
        await context.SaveChangesAsync(cancellationToken);
    }

    public static (bool validationOk, string serializedOutput, CsvOutputResult? csvOutput) ParseImportFile(
        ImportOrganisationsStatusListItem importFile)
    {
        var parsedRecords = ImportFileParser.Parse(importFile.FileContent).ToList();
        var validationIssues = RecordValidator.Validate(parsedRecords).ToList();
        if (validationIssues.Any())
        {
            var csvIssueOutput = CsvOutputResult.WithIssues(validationIssues);
            return (false, OutputSerializer.Serialize(csvIssueOutput), csvIssueOutput);
        }

        var csvRecordOutput =
            CsvOutputResult.WithRecords(parsedRecords.Select(r => OutputRecord.From(r.OutputRecord!)));
        return (true, importFile.FileContent, csvRecordOutput);
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

    private static bool GetSuccess()
    {
        var rnd = new Random();

        var value = rnd.NextInt64(0, 10);

        return value != 5;
    }

    private static async Task DoDelay(CancellationToken cancellationToken)
    {
        var rnd = new Random();

        var delay = (int)rnd.NextInt64(1, 60);

        await Task.Delay(delay * 1000, cancellationToken);
    }
}
