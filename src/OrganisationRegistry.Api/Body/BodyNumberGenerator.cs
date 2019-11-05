namespace OrganisationRegistry.Api.Body
{
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body;

    public class BodyNumberGenerator : NumberGenerator, IBodyNumberGenerator
    {
        public BodyNumberGenerator(OrganisationRegistryContext context) : base("ORG", "Orgaan nummer", () => context.BodyDetail.Max(item => item.BodyNumber))
        {}
    }
}
