namespace OrganisationRegistry.Api.ScheduledCommands;

using System;
using System.Threading;
using System.Threading.Tasks;
using Backoffice.Admin.Task;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using SqlServer;

public class SyncFromKboService : BackgroundService
{
    private const int IntervalSeconds = 3600;

    private static int _runCounter;
    private readonly IContextFactory _contextFactory;
    private readonly IKboSync _kboSync;
    private readonly ILogger<SyncFromKboService> _logger;
    private readonly ICommandSender _sender;

    public SyncFromKboService(
        IContextFactory contextFactory,
        ICommandSender sender,
        IKboSync kboSync,
        ILogger<SyncFromKboService> logger)
    {
        _contextFactory = contextFactory;
        _sender = sender;
        _kboSync = kboSync;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (Interlocked.CompareExchange(ref _runCounter, 1, 0) != 0)
            return;

        try
        {
            await Process(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while processing scheduled commands");
        }
        finally
        {
            Interlocked.Decrement(ref _runCounter);
        }
    }

    private async Task Process(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogDebug("Starting KBO Sync");

            await using var context = _contextFactory.Create();
            await _kboSync.SyncFromKbo(_sender, context, WellknownUsers.KboSyncService);
            _logger.LogInformation("KBO Synced successfully");

            await DelaySeconds(IntervalSeconds, cancellationToken);
        }
    }

    private static Task DelaySeconds(int intervalSeconds, CancellationToken cancellationToken)
        => Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
}
