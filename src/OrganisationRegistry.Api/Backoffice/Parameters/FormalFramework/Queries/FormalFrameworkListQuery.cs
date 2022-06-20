namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using SqlServer.FormalFramework;
using SqlServer.Infrastructure;

public class FormalFrameworkListQueryResult
{
    public Guid Id { get; }
    public string Code { get; }
    public string FormalFrameworkCategoryName { get; }
    public string Name { get; }

    public FormalFrameworkListQueryResult(
        Guid id,
        string code,
        string formalFrameworkCategoryName,
        string name)
    {
        Id = id;
        Code = code;
        FormalFrameworkCategoryName = formalFrameworkCategoryName;
        Name = name;
    }
}

public class FormalFrameworkListQuery : Query<FormalFrameworkListItem, FormalFrameworkListItemFilter, FormalFrameworkListQueryResult>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new FormalFrameworkListSorting();

    protected override Expression<Func<FormalFrameworkListItem, FormalFrameworkListQueryResult>> Transformation =>
        x => new FormalFrameworkListQueryResult(
            x.Id,
            x.Code,
            x.FormalFrameworkCategoryName,
            x.Name);

    public FormalFrameworkListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<FormalFrameworkListItem> Filter(FilteringHeader<FormalFrameworkListItemFilter> filtering)
    {
        var formalFrameworks = _context.FormalFrameworkList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return formalFrameworks;

        if (!filter.Ids.IsNullOrEmpty())
            formalFrameworks = formalFrameworks.Where(x => filter.Ids.Contains(x.Id));

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            formalFrameworks = formalFrameworks.Where(x => x.Name.Contains(name));

        if (filter.Code is { } code && code.IsNotEmptyOrWhiteSpace())
            formalFrameworks = formalFrameworks.Where(x => x.Code.Contains(code));

        if (filter.FormalFrameworkCategoryName is { } formalFrameworkCategoryName && formalFrameworkCategoryName.IsNotEmptyOrWhiteSpace())
            formalFrameworks = formalFrameworks.Where(x => x.FormalFrameworkCategoryName.Contains(formalFrameworkCategoryName));

        return formalFrameworks;
    }

    private class FormalFrameworkListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(FormalFrameworkListItem.Name),
            nameof(FormalFrameworkListItem.Code),
            nameof(FormalFrameworkListItem.FormalFrameworkCategoryName),
        };

        public SortingHeader DefaultSortingHeader { get; } = new(nameof(FormalFrameworkListItem.Name), SortOrder.Ascending);
    }
}

public class FormalFrameworkListItemFilter
{
    public IList<Guid> Ids { get; set; } = new List<Guid>();

    public string? Name { get; set; }

    public string? Code { get; set; }

    public string? FormalFrameworkCategoryName { get; set; }
}
