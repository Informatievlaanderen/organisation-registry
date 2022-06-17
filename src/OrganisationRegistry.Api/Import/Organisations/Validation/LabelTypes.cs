namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.SqlServer.Infrastructure;

public static class LabelTypes
{
    public static async Task<ImmutableList<string>> Get(OrganisationRegistryContext context)
        => (await GetLabelTypes(context)).ToImmutableList();

    private static async Task<List<string>> GetLabelTypes(OrganisationRegistryContext context)
        => await context.LabelTypeList.Select(labelType => labelType.Name).ToListAsync();
}
