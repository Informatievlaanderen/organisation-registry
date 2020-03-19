namespace OrganisationRegistry.Api.Task
{
    using System;
    using System.Linq;
    using System.Security.Claims;
    using Configuration;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Infrastructure;

    public interface IKboSync
    {
        void SyncFromKbo(
            ICommandSender commandSender,
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

        public KboSync(
            IDateTimeProvider dateTimeProvider,
            IOptions<ApiConfiguration> apiOptions)
        {
            _dateTimeProvider = dateTimeProvider;
            _syncFromKboBatchSize = apiOptions.Value.SyncFromKboBatchSize;
        }

        public void SyncFromKbo(
            ICommandSender commandSender,
            OrganisationRegistryContext context,
            ClaimsPrincipal claimsPrincipal)
        {
            var itemsInQueue = context.KboSyncQueue
                .Where(item => item.SyncCompletedAt == null)
                .OrderBy(item => item.MutationReadAt)
                .ThenBy(item => item.SourceOrganisationKboNumber)
                .Take(_syncFromKboBatchSize);

            foreach (var kboSyncQueueItem in itemsInQueue)
            {
                try
                {
                    var organisationDetailItem = context.OrganisationDetail.SingleOrDefault(item => item.KboNumber == kboSyncQueueItem.SourceOrganisationKboNumber);
                    if (organisationDetailItem == null)
                    {
                        kboSyncQueueItem.SyncStatus = SyncStatusNotFound;
                        kboSyncQueueItem.SyncInfo = SyncInfoNotFound;
                        continue;
                    }

                    commandSender.Send(new UpdateFromKbo(new OrganisationId(organisationDetailItem.Id), claimsPrincipal, _dateTimeProvider.Today));

                    kboSyncQueueItem.SyncCompletedAt = _dateTimeProvider.UtcNow;
                    kboSyncQueueItem.SyncStatus = SyncStatusSuccess;
                }
                catch (Exception e)
                {
                    kboSyncQueueItem.SyncStatus = SyncStatusError;
                    kboSyncQueueItem.SyncInfo = e.ToString();
                }
            }

            context.SaveChanges();
        }
    }
}
