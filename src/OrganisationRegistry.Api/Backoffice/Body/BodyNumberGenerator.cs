namespace OrganisationRegistry.Api.Backoffice.Body;

using System.Linq;
using OrganisationRegistry.Body;
using SqlServer.Infrastructure;

public class BodyNumberGenerator : NumberGenerator, IBodyNumberGenerator
{
    public BodyNumberGenerator(OrganisationRegistryContext context) : base("ORG", "Orgaan nummer", () => context.BodyDetail.Max(item => item.BodyNumber) ?? "")
    {}
}