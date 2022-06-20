namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType.Queries;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.FunctionType;
using SqlServer.Infrastructure;

public class FunctionTypeListQuery : Query<FunctionTypeListItem>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new FunctionTypeListSorting();

    public FunctionTypeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<FunctionTypeListItem> Filter(FilteringHeader<FunctionTypeListItem> filtering)
    {
        var functionTypes = _context.FunctionTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return functionTypes;

        if (!filter.Name.IsNullOrWhiteSpace())
            functionTypes = functionTypes.Where(x => x.Name.Contains(filter.Name));

        return functionTypes;
    }

    private class FunctionTypeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(FunctionTypeListItem.Name),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(FunctionTypeListItem.Name), SortOrder.Ascending);
    }
}
