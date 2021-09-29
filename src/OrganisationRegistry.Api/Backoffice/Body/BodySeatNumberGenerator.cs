namespace OrganisationRegistry.Api.Backoffice.Body
{
    using System.Linq;
    using OrganisationRegistry.Body;
    using SqlServer.Infrastructure;

    public class BodySeatNumberGenerator : NumberGenerator, IBodySeatNumberGenerator
    {
        public BodySeatNumberGenerator(OrganisationRegistryContext context) : base("POS", "Post nummer", () => context.BodySeatList.Max(item => item.BodySeatNumber))
        { }
    }
}
