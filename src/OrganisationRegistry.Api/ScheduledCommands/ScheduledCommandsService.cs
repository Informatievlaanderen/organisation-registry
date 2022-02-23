namespace OrganisationRegistry.Api.ScheduledCommands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using SqlServer;

    public class ScheduledCommandsService : BackgroundService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICommandSender _commandSender;
        private readonly ILogger<ScheduledCommandsService> _logger;
        private const int IntervalSeconds = 10;

        private static int runCounter = 0;

        public ScheduledCommandsService(IContextFactory contextFactory, IDateTimeProvider dateTimeProvider,
            ICommandSender commandSender, ILogger<ScheduledCommandsService> logger)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _commandSender = commandSender;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if (Interlocked.CompareExchange(ref runCounter, 1, 0) != 0)
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
                Interlocked.Decrement(ref runCounter);
            }
        }

        private async Task Process(CancellationToken cancellationToken)
        {
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

                    await DelaySeconds(1, cancellationToken);

                    _logger.LogDebug("Sending command: {Command}", command.GetType().FullName);

                    try
                    {
                        command.User = WellknownUsers.ScheduledCommandsServiceUser;
                        await _commandSender.Send(command);
                        _logger.LogInformation("command {@Command} sent successfully", command);
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical(e, "An error occured while processing scheduled Command: {@Command}", command);
                    }
                }

                await DelaySeconds(IntervalSeconds, cancellationToken);
            }
        }

        private static Task DelaySeconds(int intervalSeconds, CancellationToken cancellationToken) =>
            Task.Delay((int)TimeSpan.FromSeconds(intervalSeconds).TotalMilliseconds, cancellationToken);

        /// <summary>
        /// made public for testing purposes
        /// </summary>
        /// <param name="today"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ICommand>> GetCommands(DateTime today)
        {
            await using var context = _contextFactory.Create();

            var commands = new List<ICommand>();

            commands.AddRange(await context.ActiveOrganisationParentList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.FutureActiveOrganisationParentList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.ActiveBodyOrganisationList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.FutureActiveBodyOrganisationList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.ActivePeopleAssignedToBodyMandatesList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.FuturePeopleAssignedToBodyMandatesList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.ActiveOrganisationFormalFrameworkList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.FutureActiveOrganisationFormalFrameworkList.GetScheduledCommandsToExecute(today));
            commands.AddRange(await context.OrganisationCapacityList.GetScheduledCommandsToExecute(today));

            return commands;
        }
    }
}
