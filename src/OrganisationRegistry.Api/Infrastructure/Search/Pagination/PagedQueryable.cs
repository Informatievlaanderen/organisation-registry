namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

using System.Linq;

public class PagedQueryable<T>
{
    public IQueryable<T> Items { get; private set; }
    public PaginationInfo PaginationInfo { get; private set; }

    public PagedQueryable(IQueryable<T> items, PaginationInfo paginationInfo)
    {
        Items = items;
        PaginationInfo = paginationInfo;
    }
}
