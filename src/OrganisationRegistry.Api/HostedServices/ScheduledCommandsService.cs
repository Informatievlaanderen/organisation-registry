namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

            var commands = await GetCommands(today);

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

            await _configuration.Delay(cancellationToken);
        }
    }


    /// <summary>
    /// made public for testing purposes
    /// </summary>
    /// <param name="today"></param>
    /// <returns></returns>
    public async Task<IEnumerable<ICommand>> GetCommands(DateTime today)
    {
        await using var context = _contextFactory.Create();

        var commands = new List<ICommand>();

        await commands.TryAddRange(_logger, today, context.ActiveOrganisationParentList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.FutureActiveOrganisationParentList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.ActiveBodyOrganisationList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.FutureActiveBodyOrganisationList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.ActivePeopleAssignedToBodyMandatesList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.FuturePeopleAssignedToBodyMandatesList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.ActiveOrganisationFormalFrameworkList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.FutureActiveOrganisationFormalFrameworkList, ScheduledCommands.GetScheduledCommandsToExecute);
        await commands.TryAddRange(_logger, today, context.OrganisationCapacityList, ScheduledCommands.GetScheduledCommandsToExecute);

        return commands;
    }
}

public static class ListOfICommandExtensions
{
    public static async Task TryAddRange<T>(this List<ICommand> list, ILogger logger, DateTime date, DbSet<T> dbSet, Func<DbSet<T>, DateTime, Task<List<ICommand>>> getRange)
        where T : class
    {
        try
        {
            list.AddRange(await getRange(dbSet, date));
        }
        catch (Exception e)
        {
            logger.LogCritical(e, "An error occured while retrieving scheduled Commands From: {Commands}", typeof(T).Name);
        }
    }
}
