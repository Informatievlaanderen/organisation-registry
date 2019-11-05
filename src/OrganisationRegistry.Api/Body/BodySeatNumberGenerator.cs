namespace OrganisationRegistry.Api.Body
{
    using System.Linq;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body;

    public class BodySeatNumberGenerator : NumberGenerator, IBodySeatNumberGenerator
    {
        public BodySeatNumberGenerator(OrganisationRegistryContext context) : base("POS", "Post nummer", () => context.BodySeatList.Max(item => item.BodySeatNumber))
        { }
    }
}
