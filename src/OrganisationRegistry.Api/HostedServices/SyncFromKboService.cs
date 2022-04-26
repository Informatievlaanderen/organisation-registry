namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Backoffice.Admin.Task;
using OrganisationRegistry.Configuration;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;

public class SyncFromKboService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IKboSync _kboSync;
    private readonly HostedServiceConfiguration _configuration;
    private readonly ILogger<SyncFromKboService> _logger;
    private readonly ICommandSender _sender;

    public SyncFromKboService(
        IContextFactory contextFactory,
        ICommandSender sender,
        IKboSync kboSync,
        IOrganisationRegistryConfiguration configuration,
        ILogger<SyncFromKboService> logger) : base(logger)
    {
        _contextFactory = contextFactory;
        _sender = sender;
        _kboSync = kboSync;
        _configuration = configuration.HostedServices.SyncFromKboService;
        _logger = logger;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!_configuration.Enabled)
            {
                _logger.LogInformation("SyncFromKboService disabled, skipping execution");
                continue;
            }

            _logger.LogInformation("Starting KBO Sync");

            await using var context = _contextFactory.Create();
            await _kboSync.SyncFromKbo(_sender, context, WellknownUsers.KboSyncService);
            _logger.LogInformation("KBO Synced successfully");

            await DelaySeconds(_configuration.DelayInSeconds, cancellationToken);
        }
    }

    private static Task DelaySeconds(int intervalSeconds, CancellationToken cancellationToken)
        => Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
}
