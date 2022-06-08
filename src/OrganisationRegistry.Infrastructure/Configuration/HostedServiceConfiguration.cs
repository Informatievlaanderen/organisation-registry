namespace OrganisationRegistry.Infrastructure.Configuration;

using System;
using System.Threading;
using System.Threading.Tasks;

public class HostedServiceConfiguration
{
    public HostedServiceConfiguration(int delayInSeconds, bool enabled)
    {
        DelayInSeconds = delayInSeconds;
        Enabled = enabled;
    }

    public int DelayInSeconds { get; }
    public bool Enabled { get; }

    public async Task Delay(CancellationToken cancellationToken)
        => await Task.Delay(TimeSpan.FromSeconds(DelayInSeconds), cancellationToken);
}
