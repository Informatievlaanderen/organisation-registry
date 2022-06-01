namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using SqlServer.Infrastructure;
using SqlServer.RegulationSubTheme;

public class RegulationSubThemeListQueryResult
{
    public Guid Id { get; }
    public string Name { get; }
    public string RegulationThemeName { get; }

    public RegulationSubThemeListQueryResult(
        Guid id,
        string name,
        string regulationThemeName)
    {
        Id = id;
        Name = name;
        RegulationThemeName = regulationThemeName;
    }
}

public class RegulationSubThemeListQuery: Query<RegulationSubThemeListItem, RegulationSubThemeListItemFilter, RegulationSubThemeListQueryResult>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new RegulationSubThemeListSorting();

    protected override Expression<Func<RegulationSubThemeListItem, RegulationSubThemeListQueryResult>> Transformation =>
        x => new RegulationSubThemeListQueryResult(
            x.Id,
            x.Name,
            x.RegulationThemeName);

    public RegulationSubThemeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<RegulationSubThemeListItem> Filter(FilteringHeader<RegulationSubThemeListItemFilter> filtering)
    {
        var regulationSubThemes = _context.RegulationSubThemeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return regulationSubThemes;

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            regulationSubThemes = regulationSubThemes.Where(x => x.Name.Contains(name));

        if (filter.RegulationThemeName is { } regulationThemeName && regulationThemeName.IsNotEmptyOrWhiteSpace())
            regulationSubThemes = regulationSubThemes.Where(x => x.RegulationThemeName.Contains(regulationThemeName));

        if (!filter.RegulationThemeId.IsEmptyGuid())
            regulationSubThemes = regulationSubThemes.Where(x => x.RegulationThemeId == filter.RegulationThemeId);

        return regulationSubThemes;
    }

    private class RegulationSubThemeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(RegulationSubThemeListItem.Name),
            nameof(RegulationSubThemeListItem.RegulationThemeName),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(RegulationSubThemeListItem.Name), SortOrder.Ascending);
    }
}

public class RegulationSubThemeListItemFilter
{
    public string? Name { get; set; }
    public Guid RegulationThemeId { get; set; }
    public string? RegulationThemeName { get; set; }
}
