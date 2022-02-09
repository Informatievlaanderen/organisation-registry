namespace OrganisationRegistry.Configuration
{
    using System;

    public interface IOrganisationRegistryConfiguration
    {
        Guid OrafinKeyTypeId { get; }
        Guid VlimpersKeyTypeId { get; }
        string OrafinOvoCode { get; }
        Guid FormalNameLabelTypeId { get; }
        Guid FormalShortNameLabelTypeId { get; }
        public IKboConfiguration Kbo { get; }
        public IAuthorizationConfiguration Authorization { get; }
    }
}
