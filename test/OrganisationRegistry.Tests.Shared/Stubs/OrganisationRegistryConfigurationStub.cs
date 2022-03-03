namespace OrganisationRegistry.Tests.Shared.Stubs
{
    using System;
    using Configuration;

    public class OrganisationRegistryConfigurationStub : IOrganisationRegistryConfiguration
    {
        public OrganisationRegistryConfigurationStub()
        {
            Kbo = new KboConfigurationStub();
            Authorization = new AuthorizationConfigurationStub();
            Caching = new CachingConfigurationStub();
        }
        public Guid KboKeyTypeId { get; set; }
        public Guid OrafinKeyTypeId { get; set; }
        public Guid VlimpersKeyTypeId { get; set; }
        public string OrafinOvoCode { get; set; }
        public Guid FormalNameLabelTypeId { get; set; }
        public Guid FormalShortNameLabelTypeId { get; set; }
        public IKboConfiguration Kbo { get; init; }
        public IAuthorizationConfiguration Authorization { get; init; }
        public ICachingConfiguration Caching { get; init; }
    }
}
