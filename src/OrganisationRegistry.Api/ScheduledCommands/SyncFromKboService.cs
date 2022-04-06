namespace OrganisationRegistry.Api.ScheduledCommands;

using System;
using System.Threading;
using System.Threading.Tasks;
using Backoffice.Admin.Task;
using Configuration;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Configuration;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using SqlServer;

public class SyncFromKboService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IKboSync _kboSync;
    private readonly HostedServiceConfiguration _configuration;
    private readonly ILogger<SyncFromKboService> _logger;
    private readonly ICommandSender _sender;

    private static int _runCounter;

    public SyncFromKboService(
        IContextFactory contextFactory,
        ICommandSender sender,
        IKboSync kboSync,
        IOrganisationRegistryConfiguration configuration,
        ILogger<SyncFromKboService> logger)
    {
        _contextFactory = contextFactory;
        _sender = sender;
        _kboSync = kboSync;
        _configuration = configuration.HostedServices.SyncFromKboService;
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
            if (!_configuration.Enabled)
            {
                _logger.LogInformation("SyncFromKboService disabled, skipping execution");
                continue;
            }

            _logger.LogDebug("Starting KBO Sync");

            await using var context = _contextFactory.Create();
            await _kboSync.SyncFromKbo(_sender, context, WellknownUsers.KboSyncService);
            _logger.LogInformation("KBO Synced successfully");

            await DelaySeconds(_configuration.DelayInSeconds, cancellationToken);
        }
    }

    private static Task DelaySeconds(int intervalSeconds, CancellationToken cancellationToken)
        => Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
}
