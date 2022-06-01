namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

using System;
using System.Linq;

public interface IPaginationRequest
{
    PagedQueryable<T> Paginate<T>(IQueryable<T> source);
}

public class PaginationRequest : IPaginationRequest
{
    public int RequestedPage { get; }

    public int ItemsPerPage { get; }

    public PaginationRequest(int requestedPage, int itemsPerPage)
    {
        RequestedPage = requestedPage;
        ItemsPerPage = itemsPerPage;
    }

    public PagedQueryable<T> Paginate<T>(IQueryable<T> source)
    {
        var itemsInRequestedPage = source
            .Skip((RequestedPage - 1) * ItemsPerPage)
            .Take(ItemsPerPage);

        var totalItemSize = source.Count();
        var totalPages = (int)Math.Ceiling((double)totalItemSize / ItemsPerPage);
        var paginationInfo = new PaginationInfo(RequestedPage, ItemsPerPage, totalItemSize, totalPages);

        return new PagedQueryable<T>(itemsInRequestedPage, paginationInfo);
    }
}

public class NoPaginationRequest : IPaginationRequest
{
    public int TotalPages(int totalItemSize)
    {
        return 1;
    }

    public PagedQueryable<T> Paginate<T>(IQueryable<T> source)
    {
        var itemsPerPage = source.Count();
        return new PagedQueryable<T>(source, new PaginationInfo(1, itemsPerPage, itemsPerPage, 1));
    }
}
