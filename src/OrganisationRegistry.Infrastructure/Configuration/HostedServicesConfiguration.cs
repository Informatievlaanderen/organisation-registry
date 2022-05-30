namespace OrganisationRegistry.Infrastructure.Configuration;

public class HostedServicesConfiguration : IHostedServicesConfiguration
{
    public HostedServicesConfiguration(HostedServicesConfigurationSection? configuration)
    {
        SyncFromKboService = new HostedServiceConfiguration(
            configuration?.SyncFromKboService?.DelayInSeconds ?? 3600,
            configuration?.SyncFromKboService?.Enabled ?? true);

        ScheduledCommandsService = new HostedServiceConfiguration(
            configuration?.ScheduledCommandsService?.DelayInSeconds ?? 3600,
            configuration?.ScheduledCommandsService?.Enabled ?? true);

        SyncRemovedItemsService = new HostedServiceConfiguration(
            configuration?.SyncRemovedItemsService?.DelayInSeconds ?? 3600,
            configuration?.SyncRemovedItemsService?.Enabled ?? true);
    }
    public HostedServiceConfiguration SyncFromKboService { get; }
    public HostedServiceConfiguration ScheduledCommandsService { get; }
    public HostedServiceConfiguration SyncRemovedItemsService { get; }
}