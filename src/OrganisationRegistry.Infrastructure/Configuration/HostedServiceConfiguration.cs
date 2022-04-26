namespace OrganisationRegistry.Infrastructure.Configuration;

public class HostedServiceConfiguration
{
    public HostedServiceConfiguration(int delayInSeconds, bool enabled)
    {
        DelayInSeconds = delayInSeconds;
        Enabled = enabled;
    }

    public int DelayInSeconds { get; }
    public bool Enabled { get; }
}
