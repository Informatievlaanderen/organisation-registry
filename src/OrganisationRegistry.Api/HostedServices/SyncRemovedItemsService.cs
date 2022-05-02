namespace OrganisationRegistry.Api.HostedServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using Organisation.Commands;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using SqlServer;

    public class SyncRemovedItemsService : BackgroundService
    {
        private readonly IContextFactory _contextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ICommandSender _commandSender;
        private readonly ILogger<SyncRemovedItemsService> _logger;
        private readonly HostedServiceConfiguration _configuration;


        public SyncRemovedItemsService(
            IContextFactory contextFactory,
            IDateTimeProvider dateTimeProvider,
            ICommandSender commandSender,
            IOrganisationRegistryConfiguration configuration,
            ILogger<SyncRemovedItemsService> logger) : base(logger)
        {
            _contextFactory = contextFactory;
            _dateTimeProvider = dateTimeProvider;
            _commandSender = commandSender;
            _configuration = configuration.HostedServices.SyncRemovedItemsService;
            _logger = logger;
        }

        protected override async Task Process(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (!_configuration.Enabled)
                {
                    _logger.LogInformation("SyncRemovedItemsService disabled, skipping execution");
                    continue;
                }

                await using var context = _contextFactory.Create();

                var organisationKeys = context.OrganisationKeyList.Where(item => item.ScheduledForRemoval);
                foreach (var organisationKey in organisationKeys)
                {
                    var removeOrganisationKey = new RemoveOrganisationKey(
                        new OrganisationId(organisationKey.OrganisationId),
                        new OrganisationKeyId(organisationKey.OrganisationKeyId))
                    {
                        User = WellknownUsers.SyncRemovedItemsService
                    };

                    await _commandSender.Send(removeOrganisationKey);
                }

                await DelaySeconds(_configuration.DelayInSeconds, cancellationToken);
            }
        }

        private static Task DelaySeconds(int intervalSeconds, CancellationToken cancellationToken) =>
            Task.Delay(TimeSpan.FromSeconds(intervalSeconds), cancellationToken);
    }
}
