namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme.Queries;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.RegulationTheme;

public class RegulationThemeListQuery: Query<RegulationThemeListItem>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new RegulationThemeListSorting();

    public RegulationThemeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<RegulationThemeListItem> Filter(FilteringHeader<RegulationThemeListItem> filtering)
    {
        var regulationThemes = _context.RegulationThemeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return regulationThemes;

        if (!filter.Name.IsNullOrWhiteSpace())
            regulationThemes = regulationThemes.Where(x => x.Name.Contains(filter.Name));

        return regulationThemes;
    }

    private class RegulationThemeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(RegulationThemeListItem.Name)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(RegulationThemeListItem.Name), SortOrder.Ascending);
    }
}
