namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using ProcessImportedFiles.Processor;
using SqlServer;

public class ProcessImportedFilesService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProcessImportedFilesService> _logger;
    private readonly ICommandSender _commandSender;
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly HostedServiceConfiguration _hostedServiceConfiguration;

    public ProcessImportedFilesService(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger<ProcessImportedFilesService> logger,
        ICommandSender commandSender,
        IOrganisationRegistryConfiguration configuration) : base(logger)
    {
        _contextFactory = contextFactory;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
        _commandSender = commandSender;
        _configuration = configuration;
        _hostedServiceConfiguration = configuration.HostedServices.ProcessImportedFileService;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        if (!_hostedServiceConfiguration.Enabled)
        {
            _logger.LogInformation("{ServiceName} disabled, skipping execution", nameof(ProcessImportedFilesService));
            return;
        }

        _logger.LogInformation("Starting {ServiceName}", nameof(ProcessImportedFilesService));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ImportNextFileProcessor.ProcessNextFile(
                    _contextFactory,
                    _dateTimeProvider,
                    _logger,
                    _commandSender,
                    _hostedServiceConfiguration,
                    _configuration,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An exception occurred while processing next file. " +
                    "Will retry in {DelayInSeconds} seconds.",
                    _hostedServiceConfiguration.DelayInSeconds);

                await Task.Delay(_hostedServiceConfiguration.DelayInSeconds, cancellationToken);
            }
        }
    }
}
