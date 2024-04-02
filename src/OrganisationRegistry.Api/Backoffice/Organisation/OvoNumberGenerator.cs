namespace OrganisationRegistry.Api.Backoffice.Organisation;

using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Organisation;
using SqlServer.Infrastructure;

public class OvoNumberGenerator : NumberGenerator, IOvoNumberGenerator
{
    public OvoNumberGenerator(OrganisationRegistryContext context)
        : base("OVO", "Organisatie nummer", () => context.OrganisationDetail.AsNoTracking().Max(item => item.OvoNumber))
    { }
}
