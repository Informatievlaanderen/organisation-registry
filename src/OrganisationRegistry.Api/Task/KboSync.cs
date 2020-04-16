namespace OrganisationRegistry.Api.Task
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Infrastructure;

    public interface IKboSync
    {
        Task SyncFromKbo(ICommandSender commandSender,
            OrganisationRegistryContext context,
            ClaimsPrincipal claimsPrincipal);
    }

    public class KboSync : IKboSync
    {
        public const string SyncStatusSuccess = "SUCCESS";
        public const string SyncStatusNotFound = "NOT FOUND";
        public const string SyncStatusError = "ERROR";
        public const string SyncInfoNotFound = "Er werd geen organisatie gevonden voor dit kbo nummer";

        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly int _syncFromKboBatchSize;
        private readonly ILogger<KboSync> _logger;

        public KboSync(
            IDateTimeProvider dateTimeProvider,
            IOptions<ApiConfiguration> apiOptions,
            ILogger<KboSync> logger)
        {
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
            _syncFromKboBatchSize = apiOptions.Value.SyncFromKboBatchSize;
        }

        public async Task SyncFromKbo(ICommandSender commandSender,
            OrganisationRegistryContext context,
            ClaimsPrincipal claimsPrincipal)
        {
            _logger.LogInformation("Kbo sync started.");

            var itemsInQueue = context.KboSyncQueue
                .Where(item => item.SyncCompletedAt == null)
                .OrderBy(item => item.MutationReadAt)
                .ThenBy(item => item.SourceOrganisationKboNumber)
                .Take(_syncFromKboBatchSize);

            _logger.LogInformation("Found {NumberOfSyncItems} items to sync.", itemsInQueue.Count());

            foreach (var kboSyncQueueItem in itemsInQueue)
            {
                try
                {
                    _logger.LogInformation("Trying to sync for kbo number {KboNumber}.", kboSyncQueueItem.SourceOrganisationKboNumber);

                    var organisationDetailItem = context.OrganisationDetail.SingleOrDefault(item => item.KboNumber == kboSyncQueueItem.SourceOrganisationKboNumber);
                    if (organisationDetailItem == null)
                    {
                        kboSyncQueueItem.SyncStatus = SyncStatusNotFound;
                        kboSyncQueueItem.SyncInfo = SyncInfoNotFound;

                        _logger.LogWarning("Organisation for kbo number {KboNumber} not found.", kboSyncQueueItem.SourceOrganisationKboNumber);

                        continue;
                    }

                    await commandSender.Send(new UpdateFromKbo(
                        new OrganisationId(organisationDetailItem.Id),
                        claimsPrincipal,
                        _dateTimeProvider.Today,
                        kboSyncQueueItem.Id));

                    kboSyncQueueItem.SyncCompletedAt = _dateTimeProvider.UtcNow;
                    kboSyncQueueItem.SyncStatus = SyncStatusSuccess;
                    kboSyncQueueItem.SyncInfo = string.Empty;

                    _logger.LogInformation("Kbo sync for kbo number {KboNumber} completed.", kboSyncQueueItem.SourceOrganisationKboNumber);
                }
                catch (Exception e)
                {
                    kboSyncQueueItem.SyncStatus = SyncStatusError;
                    kboSyncQueueItem.SyncInfo = e.ToString();
                    _logger.LogError(e, e.Message);
                }
            }

            context.SaveChanges();

            _logger.LogInformation("Kbo sync completed.");
        }
    }
}
