namespace OrganisationRegistry.Tests.Shared.Stubs;

using Configuration;

public class HostedServicesConfigurationStub : IHostedServicesConfiguration
{
    public HostedServicesConfigurationStub()
    {
        SyncFromKboService = new HostedServiceConfiguration(60, false);
        ScheduledCommandsService = new HostedServiceConfiguration(60, false);
    }
    public HostedServiceConfiguration SyncFromKboService { get; }
    public HostedServiceConfiguration ScheduledCommandsService { get; }
}
