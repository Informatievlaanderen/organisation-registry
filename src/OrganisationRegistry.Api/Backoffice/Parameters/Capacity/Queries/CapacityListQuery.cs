namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using Parameters;
using SqlServer.Capacity;
using SqlServer.Infrastructure;

public class CapacityListItemResult
{
    public Guid Id { get; }
    public string Name { get; }
    public bool IsRemoved { get; }
    public ResourceSelectPermissions Permissions { get; }

    public CapacityListItemResult(
        Guid id,
        string name,
        bool isRemoved,
        Func<Guid, bool> isAuthorizedForCapacity)
    {
        Id = id;
        Name = name;
        IsRemoved = isRemoved;
        Permissions = new ResourceSelectPermissions(isAuthorizedForCapacity(id));
    }
}

public class CapacityListQuery: Query<CapacityListItem, CapacityListQuery.CapacityListFilter, CapacityListItemResult>
{
    public class CapacityListFilter
    {
        public string? Name { get; set; }
        public bool ShowAll { get; set; } = false;
    }

    private readonly OrganisationRegistryContext _context;
    private readonly Func<Guid, bool> _isAuthorizedForCapacity;

    protected override ISorting Sorting => new CapacityListSorting();

    protected override Expression<Func<CapacityListItem, CapacityListItemResult>> Transformation =>
        x => new CapacityListItemResult(
            x.Id,
            x.Name,
            x.IsRemoved,
            _isAuthorizedForCapacity);

    public CapacityListQuery(OrganisationRegistryContext context, Func<Guid, bool> isAuthorizedForCapacity)
    {
        _context = context;
        _isAuthorizedForCapacity = isAuthorizedForCapacity;
    }

    protected override IQueryable<CapacityListItem> Filter(FilteringHeader<CapacityListFilter> filtering)
    {
        var capacities = _context.CapacityList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return capacities.Where(x => !x.IsRemoved);

        if (!filter.ShowAll)
            capacities = capacities.Where(x => !x.IsRemoved);

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            capacities = capacities.Where(x => x.Name.Contains(name));

        return capacities;
    }

    private class CapacityListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(CapacityListItem.Name),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(CapacityListItem.Name), SortOrder.Ascending);
    }
}
