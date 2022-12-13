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
    private readonly HostedServiceConfiguration _configuration;

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
        _configuration = configuration.HostedServices.ProcessImportedFileService;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        if (!_configuration.Enabled)
        {
            _logger.LogInformation($"{nameof(ProcessImportedFilesService)} disabled, skipping execution");
            return;
        }

        _logger.LogInformation($"Starting {nameof(ProcessImportedFilesService)}");

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await ImportNextFileProcessor.ProcessNextFile(
                    _contextFactory,
                    _dateTimeProvider,
                    _logger,
                    _commandSender,
                    _configuration,
                    cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "An exception occurred while processing next file. " +
                    "Will retry in {DelayInSeconds} seconds.",
                    _configuration.DelayInSeconds);

                await Task.Delay(_configuration.DelayInSeconds, cancellationToken);
            }
        }
    }
}
