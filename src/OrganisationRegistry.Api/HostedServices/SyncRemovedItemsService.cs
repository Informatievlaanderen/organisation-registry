namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Organisation;
using Organisation.Commands;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;
using SqlServer.Organisation;

public class SyncRemovedItemsService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly ICommandSender _commandSender;
    private readonly ILogger<SyncRemovedItemsService> _logger;
    private readonly HostedServiceConfiguration _configuration;


    public SyncRemovedItemsService(
        IContextFactory contextFactory,
        ICommandSender commandSender,
        IOrganisationRegistryConfiguration configuration,
        ILogger<SyncRemovedItemsService> logger) : base(logger)
    {
        _contextFactory = contextFactory;
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
                await _configuration.Delay(cancellationToken);
                continue;
            }

            await RemoveOrganisationItems(_commandSender, _contextFactory);

            await _configuration.Delay(cancellationToken);
        }
    }

    private static async Task RemoveOrganisationItems(ICommandSender commandSender, IContextFactory contextFactory)
    {
        await using var context = contextFactory.Create();

        await RemoveOrganisationItems(
            commandSender.Send,
            context.OrganisationKeyList,
            item => new RemoveOrganisationKey(
                new OrganisationId(item.OrganisationId),
                new OrganisationKeyId(item.OrganisationKeyId)
            ));
        await RemoveOrganisationItems(
            commandSender.Send,
            context.OrganisationCapacityList,
            item => new RemoveOrganisationCapacity(
                new OrganisationId(item.OrganisationId),
                new OrganisationCapacityId(item.OrganisationCapacityId)
            ));
    }

    private static async Task RemoveOrganisationItems<TItem, TCommand>(Func<TCommand, IUser?, Task> sendCommand, IQueryable<TItem> list, Func<TItem, TCommand> createCommand)
        where TItem : IRemovable
        where TCommand : ICommand
    {
        var organisationItems = list.Where(item => item.ScheduledForRemoval);
        foreach (var organisationItem in organisationItems)
        {
            var removeOrganisationItem = createCommand(organisationItem);

            await sendCommand(removeOrganisationItem, WellknownUsers.SyncRemovedItemsService);
        }
    }
}
