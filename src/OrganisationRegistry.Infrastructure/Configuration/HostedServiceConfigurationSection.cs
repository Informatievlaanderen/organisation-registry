namespace OrganisationRegistry.Infrastructure.Configuration;

public class HostedServiceConfigurationSection
{
    public int? DelayInSeconds { get; set; }
    public bool? Enabled { get; set; }
}
