namespace OrganisationRegistry.Configuration;

public interface IHostedServicesConfiguration
{
    public HostedServiceConfiguration SyncFromKboService { get; }
    public HostedServiceConfiguration ScheduledCommandsService { get; }
}
