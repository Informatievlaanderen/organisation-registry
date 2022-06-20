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
    protected ImportCache(
        IEnumerable<OrganisationListItem> organisations,
        Dictionary<string, (Guid id, string name)> labelTypes)
    {
        LabelTypes = labelTypes;
        OrganisationsCache = organisations.ToImmutableList();
    }

    public Dictionary<string, (Guid id, string name)> LabelTypes { get; }
    public ImmutableList<OrganisationListItem> OrganisationsCache { get; }

    public static ImportCache Create(
        OrganisationRegistryContext context,
        IEnumerable<ParsedRecord> parsedRecords)
        => new(
            GetOrganisationsInScope(context, parsedRecords).AsNoTracking(),
            GetLabelTypes(context));

    private static IQueryable<OrganisationListItem> GetOrganisationsInScope(OrganisationRegistryContext context, IEnumerable<ParsedRecord> parsedRecords)
    {
        var parentOvoNumbers = parsedRecords
            .Select(parsedRecord => parsedRecord.OutputRecord)
            .Select(outputRecord => outputRecord?.Parent.Value)
            .Where(ovoNumber => !string.IsNullOrWhiteSpace(ovoNumber))
            .Select(ovoNumber => ovoNumber!.ToLower())
            .Distinct()
            .ToList();

        return context.OrganisationList
            .Where(org => org.FormalFrameworkId == null)
            .Where(
                org => parentOvoNumbers.Contains(org.OvoNumber) ||
                       parentOvoNumbers.Contains(org.ParentOrganisationOvoNumber!));
    }

    private static Dictionary<string, (Guid id, string name)> GetLabelTypes(OrganisationRegistryContext context)
        => context.LabelTypeList
            .AsNoTracking()
            .ToDictionary(type => type.Name.ToLowerInvariant(), type => (id: type.Id, name: type.Name));

    public OrganisationListItem? GetOrganisationByOvoNumber(string ovoNumber)
        => OrganisationsCache.SingleOrDefault(
            org => string.Equals(org.OvoNumber, ovoNumber, StringComparison.InvariantCultureIgnoreCase));
}
