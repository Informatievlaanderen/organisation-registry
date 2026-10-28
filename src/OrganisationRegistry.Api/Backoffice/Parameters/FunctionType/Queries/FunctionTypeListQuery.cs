namespace OrganisationRegistry.Api.Backoffice.Parameters.FunctionType.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using SqlServer.FunctionType;
using SqlServer.Infrastructure;

public class FunctionTypeListItemResult
{
    public Guid Id { get; }
    public string Name { get; }
    public bool IsRemoved { get; }

    public FunctionTypeListItemResult(Guid id, string name, bool isRemoved)
    {
        Id = id;
        Name = name;
        IsRemoved = isRemoved;
    }
}

public class FunctionTypeListQuery : Query<FunctionTypeListItem, FunctionTypeListQuery.FunctionTypeListFilter, FunctionTypeListItemResult>
{
    public class FunctionTypeListFilter
    {
        public string? Name { get; set; }
        public bool ShowAll { get; set; } = false;
    }

    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new FunctionTypeListSorting();

    protected override Expression<Func<FunctionTypeListItem, FunctionTypeListItemResult>> Transformation =>
        x => new FunctionTypeListItemResult(
            x.Id,
            x.Name,
            x.IsRemoved);

    public FunctionTypeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<FunctionTypeListItem> Filter(FilteringHeader<FunctionTypeListFilter> filtering)
    {
        var functionTypes = _context.FunctionTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return functionTypes.Where(x => !x.IsRemoved);

        if (!filter.ShowAll)
            functionTypes = functionTypes.Where(x => !x.IsRemoved);

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            functionTypes = functionTypes.Where(x => x.Name.Contains(name));

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
