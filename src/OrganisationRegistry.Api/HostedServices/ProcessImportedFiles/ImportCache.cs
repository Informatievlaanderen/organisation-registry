namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Organisation;

public class ImportCache
{
    protected ImportCache(IEnumerable<OrganisationListItem> organisations)
    {
        OrganisationsCache = organisations.ToImmutableList();
    }

    public ImmutableList<OrganisationListItem> OrganisationsCache { get; }

    public static ImportCache Create(OrganisationRegistryContext context, List<ParsedRecord> parsedRecords, DateTime today)
    {
        var parentOvoNumbers = parsedRecords
            .Select(parsedRecord => parsedRecord.OutputRecord)
            .Select(outputRecord => outputRecord?.Parent.Value)
            .Where(ovoNumber => !string.IsNullOrWhiteSpace(ovoNumber))
            .Select(ovoNumber => ovoNumber!.ToLower())
            .Distinct()
            .ToList();

        var organisationsInScope = context.OrganisationList
            .Where(org => org.FormalFrameworkId == null)
            .Where(
                org => parentOvoNumbers.Contains(org.OvoNumber) ||
                       parentOvoNumbers.Contains(org.ParentOrganisationOvoNumber!));

        return new ImportCache(
            organisationsInScope
                .AsNoTracking());
    }

    public OrganisationListItem? GetOrganisationByOvoNumber(string ovoNumber)
        => OrganisationsCache.SingleOrDefault(org => string.Equals(org.OvoNumber, ovoNumber, StringComparison.InvariantCultureIgnoreCase));
}
