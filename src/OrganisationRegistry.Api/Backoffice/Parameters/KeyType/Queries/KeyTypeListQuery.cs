namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.KeyType;

public class KeyTypeListQuery :
    Query<KeyTypeListItem, KeyTypeListQuery.KeyTypeListItemFilter, KeyTypeListItemResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Func<Guid, bool> _isAuthorizedForKeyType;

    public KeyTypeListQuery(OrganisationRegistryContext context, Func<Guid, bool> isAuthorizedForKeyType)
    {
        _context = context;
        _isAuthorizedForKeyType = isAuthorizedForKeyType;
    }

    protected override ISorting Sorting
        => new KeyTypeListSorting();

    protected override Expression<Func<KeyTypeListItem, KeyTypeListItemResult>> Transformation
        => x => new KeyTypeListItemResult(
            x.Id,
            x.Name,
            x.IsRemoved,
            _isAuthorizedForKeyType);

    protected override IQueryable<KeyTypeListItem> Filter(FilteringHeader<KeyTypeListItemFilter> filtering)
    {
        var keyTypes = _context.KeyTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return keyTypes;

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            keyTypes = keyTypes.Where(x => x.Name.Contains(name));

        if (filter.ExcludeIds.Any())
            keyTypes = keyTypes.Where(x => !filter.ExcludeIds.Contains(x.Id));

        return keyTypes;
    }

    public class KeyTypeListItemFilter
    {
        public Guid Id { get; set; }

        public string? Name { get; set; }

        public List<Guid> ExcludeIds { get; } = new();
    }

    private class KeyTypeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(KeyTypeListItem.Name)
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(KeyTypeListItem.Name), SortOrder.Ascending);
    }
}

public class KeyTypeListItemResult
{
    public KeyTypeListItemResult(Guid id, string name, bool isRemoved, Func<Guid, bool> isAuthorizedForKeyType)
    {
        Id = id;
        Name = name;
        IsRemoved = isRemoved;
        UserPermitted = isAuthorizedForKeyType(id);
    }

    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsRemoved { get; set; }
    public bool UserPermitted { get; set; }
}
