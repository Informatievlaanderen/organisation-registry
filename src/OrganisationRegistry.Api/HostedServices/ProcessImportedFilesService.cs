namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SqlServer;
using SqlServer.Import.Organisations;

public class ProcessImportedFilesService: BackgroundService
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
            // await ProcessNextFile(_contextFactory, _dateTimeProvider, _logger, cancellationToken);
            await Task.Delay(15000, cancellationToken); // todo: move to config
        }
    }

    private static async Task ProcessNextFile(IContextFactory contextFactory, IDateTimeProvider dateTimeProvider, ILogger logger, CancellationToken cancellationToken)
    {
        // 0) context ophalen
        var context = contextFactory.Create();

        // 1) ophalen van oudste importfile in status 'Processing'
        var maybeImportFile = context.ImportOrganisationsStatusList
            .Where(listItem => listItem.Status == ImportProcessStatus.Processing)
            .MinBy(listItem => listItem.UploadedAt);

        if (maybeImportFile is not { } importFile)
            return;

        // 2) verwerken (voorlopig random 1-60 sec delay, 1/10 kans op failure ?)
        await ProcessImportFile(dateTimeProvider, importFile, logger, cancellationToken);

        // 3) resultaat van verwerking terug saven naar db
        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task ProcessImportFile(IDateTimeProvider dateTimeProvider, ImportOrganisationsStatusListItem importFile, ILogger logger, CancellationToken cancellationToken)
    {
        await DoDelay(cancellationToken);
        var success = GetSuccess();

        if (!success)
            logger.LogError("An error occured while processing the imported file {FileName}", importFile.FileName);

        importFile.Status = success ? ImportProcessStatus.Processed : ImportProcessStatus.Failed;
        importFile.LastProcessedAt = dateTimeProvider.UtcNow;
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

        await Task.Delay(delay*1000, cancellationToken);
    }
}
