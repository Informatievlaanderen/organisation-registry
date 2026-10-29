namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Organisation;
using Organisation.Commands;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;
using SqlServer.Body;
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
        if (!_configuration.Enabled)
        {
            _logger.LogInformation($"{nameof(SyncRemovedItemsService)} disabled, skipping execution");
            return;
        }

        while (!cancellationToken.IsCancellationRequested)
        {
            await RemoveOrganisationItems(_commandSender, _contextFactory, _logger);

            await _configuration.Delay(cancellationToken);
        }
    }

    private static async Task RemoveOrganisationItems(ICommandSender commandSender, IContextFactory contextFactory, ILogger logger)
    {
        await using var context = contextFactory.Create();

        await RemoveOrganisationItems(
            commandSender.Send,
            logger,
            context.OrganisationKeyList,
            item => new RemoveOrganisationKey(
                new OrganisationId(item.OrganisationId),
                new OrganisationKeyId(item.OrganisationKeyId)
            ));
        await RemoveOrganisationItems(
            commandSender.Send,
            logger,
            context.OrganisationCapacityList,
            item => new RemoveOrganisationCapacity(
                new OrganisationId(item.OrganisationId),
                new OrganisationCapacityId(item.OrganisationCapacityId)
            ));
        await RemoveOrganisationItems(
            commandSender.Send,
            logger,
            context.OrganisationFunctionList,
            item => new RemoveOrganisationFunction(
                new OrganisationId(item.OrganisationId),
                item.OrganisationFunctionId
            ));
        await RemoveOrganisationItems(
            commandSender.Send,
            logger,
            context.BodyMandateList,
            item => new RemoveBodyMandate(
                new BodyId(item.BodyId),
                new BodySeatId(item.BodySeatId),
                new BodyMandateId(item.BodyMandateId)
            ));
    }

    private static async Task RemoveOrganisationItems<TItem, TCommand>(
        Func<TCommand, IUser?, Task> sendCommand,
        ILogger logger,
        IQueryable<TItem> list,
        Func<TItem, TCommand> createCommand)
        where TItem : IRemovable
        where TCommand : ICommand
    {
        var organisationItems = list.Where(item => item.ScheduledForRemoval);
        foreach (var organisationItem in organisationItems)
        {
            var removeOrganisationItem = createCommand(organisationItem);

            try
            {
                await sendCommand(removeOrganisationItem, WellknownUsers.SyncRemovedItemsService);
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Failed to remove scheduled {ItemType} item, will retry on next pass",
                    typeof(TItem).Name);
            }
        }
    }
}
