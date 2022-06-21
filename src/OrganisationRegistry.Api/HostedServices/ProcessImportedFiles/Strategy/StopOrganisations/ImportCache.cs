namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Organisation;

public class ImportCache
{
    protected ImportCache(IEnumerable<OrganisationDetailItem> organisations)
    {
        OrganisationsCache = organisations.ToImmutableList();
    }

    public ImmutableList<OrganisationDetailItem> OrganisationsCache { get; }

    public static ImportCache Create(OrganisationRegistryContext context, IEnumerable<ParsedRecord> parsedRecords)
        => new(GetOrganisationsInScope(context, parsedRecords).AsNoTracking());

    private static IQueryable<OrganisationDetailItem> GetOrganisationsInScope(OrganisationRegistryContext context, IEnumerable<ParsedRecord> parsedRecords)
    {
        var ovoNumbers = parsedRecords
            .Select(parsedRecord => parsedRecord.OutputRecord)
            .Select(outputRecord => outputRecord?.OvoNumber.Value)
            .Where(ovoNumber => !string.IsNullOrWhiteSpace(ovoNumber))
            .Select(ovoNumber => ovoNumber!.ToLower())
            .Distinct()
            .ToList();

        return context.OrganisationDetail
            .Where(org => org.FormalFrameworkId == null)  // todo: klopt deze check hier ?
            .Where(org => ovoNumbers.Contains(org.OvoNumber));
    }

    public OrganisationDetailItem? GetOrganisationByOvoNumber(string ovoNumber)
        => OrganisationsCache.SingleOrDefault(
            org => string.Equals(org.OvoNumber, ovoNumber, StringComparison.InvariantCultureIgnoreCase));
}
