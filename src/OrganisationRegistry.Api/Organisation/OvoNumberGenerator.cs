namespace OrganisationRegistry.Api.Organisation
{
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Organisation;

    public class OvoNumberGenerator : NumberGenerator, IOvoNumberGenerator
    {
        public OvoNumberGenerator(OrganisationRegistryContext context) : base("OVO", "Organisatie nummer", () => context.OrganisationDetail.Max(item => item.OvoNumber))
        { }
    }
}
