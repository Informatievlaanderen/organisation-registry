namespace OrganisationRegistry.Api.Backoffice.Parameters.Building.Queries;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using SqlServer.Building;
using SqlServer.Infrastructure;

public class BuildingListQuery: Query<BuildingListItem, BuildingListItemFilter>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new BuildingListSorting();

    public BuildingListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<BuildingListItem> Filter(FilteringHeader<BuildingListItemFilter> filtering)
    {
        var buildings = _context.BuildingList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return buildings;

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            buildings = buildings.Where(x => x.Name.Contains(name));

        if (filter.VimId is not { } unparsedVimId || unparsedVimId.IsNullOrWhiteSpace())
            return buildings;

        // When somebody entered a non numeric VimId, since they all need to be numeric, you get no results!
        return int.TryParse(unparsedVimId, out var vimId)
            ? buildings.Where(x => x.VimId == vimId)
            : new List<BuildingListItem>().AsAsyncQueryable();
    }

    private class BuildingListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(BuildingListItem.Name),
            nameof(BuildingListItem.VimId)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(BuildingListItem.Name), SortOrder.Ascending);
    }
}

public class BuildingListItemFilter
{
    public string? Name { get; set; }
    public string? VimId { get; set; }
}
