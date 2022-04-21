namespace OrganisationRegistry.Api.Backoffice.Admin.Task
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Infrastructure;
    using SqlServer.KboSyncQueue;

    public interface IKboSync
    {
        Task SyncFromKbo(
            ICommandSender commandSender,
            OrganisationRegistryContext context,
            IUser user);
    }

    public class KboSync : IKboSync
    {
        public const string SyncStatusSuccess = "SUCCESS";
        public const string SyncStatusNotFound = "NOT FOUND";
        public const string SyncStatusError = "ERROR";
        public const string SyncInfoNotFound = "Er werd geen organisatie gevonden voor dit KBO nummer";

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly int _syncFromKboBatchSize;
        private readonly ILogger<KboSync> _logger;

        public KboSync(
            IDateTimeProvider dateTimeProvider,
            IOptions<ApiConfigurationSection> apiOptions,
            ILogger<KboSync> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _syncFromKboBatchSize = apiOptions.Value.SyncFromKboBatchSize;
        }

        public async Task SyncFromKbo(
            ICommandSender commandSender,
            OrganisationRegistryContext context,
            IUser user)
        {
            _logger.LogInformation("KBO sync started");

            var itemsInQueue = await GetNewItems(context);

            if (!itemsInQueue.Any())
                itemsInQueue = await GetItemsToRetry(context);

            _logger.LogInformation("Found {NumberOfSyncItems} items to sync", itemsInQueue.Count);

            foreach (var kboSyncQueueItem in itemsInQueue)
            {
                try
                {
                    _logger.LogInformation("Trying to sync for KBO number {KboNumber}", kboSyncQueueItem.SourceOrganisationKboNumber);

                    var organisationDetailItem = context.OrganisationDetail.SingleOrDefault(item => item.KboNumber == kboSyncQueueItem.SourceOrganisationKboNumber);
                    if (organisationDetailItem == null)
                    {
                        kboSyncQueueItem.SyncStatus = SyncStatusNotFound;
                        kboSyncQueueItem.SyncInfo = SyncInfoNotFound;

                        _logger.LogWarning("Organisation for KBO number {KboNumber} not found", kboSyncQueueItem.SourceOrganisationKboNumber);

                        continue;
                    }

                    var syncOrganisationWithKbo = new SyncOrganisationWithKbo(
                        new OrganisationId(organisationDetailItem.Id),
                        _dateTimeProvider.Today,
                        kboSyncQueueItem.Id);

                    syncOrganisationWithKbo.User = user;

                    await commandSender.Send(syncOrganisationWithKbo);

                    kboSyncQueueItem.SyncCompletedAt = _dateTimeProvider.UtcNow;
                    kboSyncQueueItem.SyncStatus = SyncStatusSuccess;
                    kboSyncQueueItem.SyncInfo = string.Empty;

                    _logger.LogInformation("KBO sync for KBO number {KboNumber} completed", kboSyncQueueItem.SourceOrganisationKboNumber);
                }
                catch (Exception e)
                {
                    kboSyncQueueItem.SyncStatus = SyncStatusError;
                    kboSyncQueueItem.SyncInfo = e.ToString();
                    _logger.LogError(e, "Error while syncing {SyncItem}", kboSyncQueueItem.Id);
                }
            }

            await context.SaveChangesAsync();

            _logger.LogInformation("KBO sync completed");
        }

        private async Task<List<KboSyncQueueItem>> GetNewItems(OrganisationRegistryContext context)
        {
            return await context.KboSyncQueue
                .AsQueryable()
                .Where(item => item.SyncCompletedAt == null && item.SyncStatus == null)
                .OrderBy(item => item.MutationReadAt)
                .ThenBy(item => item.SourceOrganisationKboNumber)
                .Take(_syncFromKboBatchSize)
                .ToListAsync();
        }

        private async Task<List<KboSyncQueueItem>> GetItemsToRetry(OrganisationRegistryContext context)
        {
            return await context.KboSyncQueue
                .AsQueryable()
                .Where(item => item.SyncCompletedAt == null)
                .OrderBy(item => item.MutationReadAt)
                .ThenBy(item => item.SourceOrganisationKboNumber)
                .Take(_syncFromKboBatchSize)
                .ToListAsync();
        }
    }
}
