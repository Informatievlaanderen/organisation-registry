namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;

    public interface IOrganisationRegistryConfiguration
    {
        Guid OrafinKeyTypeId { get; }
        Guid VlimpersKeyTypeId { get; }
        string OrafinOvoCode { get; }
        Guid FormalNameLabelTypeId { get; }
        Guid FormalShortNameLabelTypeId { get; }
        IKboConfiguration Kbo { get; }
        IAuthorizationConfiguration Authorization { get; }
        ICachingConfiguration Caching { get; }
        IHostedServicesConfiguration HostedServices { get; }
    }
}
