namespace OrganisationRegistry.Api.HostedServices;

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using ProcessImportedFiles.Strategy;
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

        while (!cancellationToken.IsCancellationRequested)
        {
            await ImportFileProcessor.ProcessNextFile(
                _contextFactory,
                _dateTimeProvider,
                _logger,
                _commandSender,
                _configuration,
                cancellationToken);
        }
    }
}
