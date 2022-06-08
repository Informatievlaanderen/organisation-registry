namespace OrganisationRegistry.Infrastructure.Configuration;

public interface IHostedServicesConfiguration
{
    public HostedServiceConfiguration SyncFromKboService { get; }
    public HostedServiceConfiguration ScheduledCommandsService { get; }
    public HostedServiceConfiguration SyncRemovedItemsService { get; }
    public HostedServiceConfiguration ProcessImportedFileService { get; }
}
