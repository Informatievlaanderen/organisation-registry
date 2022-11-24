namespace OrganisationRegistry.Tests.Shared.Stubs;

using Infrastructure.Configuration;

public class HostedServicesConfigurationStub : IHostedServicesConfiguration
{
    public HostedServicesConfigurationStub()
    {
        SyncFromKboService = new HostedServiceConfiguration(60, false);
        ScheduledCommandsService = new HostedServiceConfiguration(60, false);
        SyncRemovedItemsService = new HostedServiceConfiguration(60, false);
        ProcessImportedFileService = new HostedServiceConfiguration(60, false);
        MEPCalculatorService = new HostedServiceConfiguration(60, false);
    }

    public HostedServiceConfiguration SyncFromKboService { get; }
    public HostedServiceConfiguration ScheduledCommandsService { get; }
    public HostedServiceConfiguration SyncRemovedItemsService { get; }
    public HostedServiceConfiguration ProcessImportedFileService { get; }
    public HostedServiceConfiguration MEPCalculatorService { get; }
}
