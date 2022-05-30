namespace OrganisationRegistry.Api.Infrastructure.Search;

using System;
using System.Linq;
using System.Linq.Expressions;
using Filtering;
using Pagination;
using Sorting;

public abstract class Query<T> : Query<T, T, T> where T : class
{
}

public abstract class Query<T, TFilter> : Query<T, TFilter, T>
    where T : class
    where TFilter : class
{
}

public abstract class Query<T, TFilter, TResult>
    where T: class
    where TFilter : class
    where TResult : class
{
    protected abstract IQueryable<T> Filter(FilteringHeader<TFilter> filtering);
    protected abstract ISorting Sorting { get; }

    protected virtual Expression<Func<T, TResult>>? Transformation => null;

    public PagedQueryable<TResult> Fetch(FilteringHeader<TFilter> filtering, SortingHeader sorting, IPaginationRequest paginationRequest)
    {
        if (filtering == null)
            throw new ArgumentNullException(nameof(filtering));

        if (sorting == null)
            throw new ArgumentNullException(nameof(sorting));

        if (paginationRequest == null)
            throw new ArgumentNullException(nameof(paginationRequest));

        var items = Filter(filtering);

        return items
            .WithSorting(sorting, Sorting)
            .WithPagination(paginationRequest)
            .WithTransformation(Transformation);
    }
}