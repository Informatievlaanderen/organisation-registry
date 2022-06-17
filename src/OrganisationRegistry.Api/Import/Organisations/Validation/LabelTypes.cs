namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.SqlServer.Infrastructure;

public static class LabelTypes
{
    public static async Task<ImmutableList<string>> GetNames(OrganisationRegistryContext context)
        => ImmutableList.Create(await context.LabelTypeList.Select(labelType => labelType.Name).ToArrayAsync());
}
