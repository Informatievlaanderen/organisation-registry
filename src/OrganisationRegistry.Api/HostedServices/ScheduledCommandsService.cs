namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;

public class ScheduledCommandsService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandSender _commandSender;
    private readonly ILogger<ScheduledCommandsService> _logger;
    private readonly HostedServiceConfiguration _configuration;


    public ScheduledCommandsService(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ICommandSender commandSender,
        IOrganisationRegistryConfiguration configuration,
        ILogger<ScheduledCommandsService> logger) : base(logger)
    {
        _contextFactory = contextFactory;
        _dateTimeProvider = dateTimeProvider;
        _commandSender = commandSender;
        _configuration = configuration.HostedServices.ScheduledCommandsService;
        _logger = logger;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        if (!_configuration.Enabled)
        {
            _logger.LogInformation($"{nameof(ScheduledCommandsService)} disabled, skipping execution");
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            var today = _dateTimeProvider.Today;
            _logger.LogDebug("Processing scheduled commands");

            await SendCommands(today, cancellationToken);

            await _configuration.Delay(cancellationToken);
        }
    }

    /// <summary>
    /// made public for testing purposes
    /// </summary>
    /// <param name="today"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task SendCommands(DateTime today, CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.Create();

        await TrySendCommands(await context.ActiveOrganisationParentList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.FutureActiveOrganisationParentList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.ActiveBodyOrganisationList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.FutureActiveBodyOrganisationList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.ActivePeopleAssignedToBodyMandatesList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.FuturePeopleAssignedToBodyMandatesList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.ActiveOrganisationFormalFrameworkList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.FutureActiveOrganisationFormalFrameworkList.GetScheduledCommandsToExecute(today), cancellationToken);
        await TrySendCommands(await context.OrganisationCapacityList.GetScheduledCommandsToExecute(today), cancellationToken);
    }

    public async Task TrySendCommands<TCommand>(List<TCommand> commands, CancellationToken cancellationToken)
        where TCommand: ICommand
    {
        try
        {
            foreach (var command in commands)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogInformation("canceling execution of scheduled tasks");
                    break;
                }

                _logger.LogDebug("Sending command: {Command}", command.GetType().FullName);

                try
                {
                    await _commandSender.Send(command, WellknownUsers.ScheduledCommandsService);
                    _logger.LogInformation("command {@Command} sent successfully", command);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "An error occured while processing scheduled Command: {@Command}", command);
                }
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "An error occured while sending scheduled Commands of type: {Commands}", typeof(TCommand).Name);
        }
    }
}
