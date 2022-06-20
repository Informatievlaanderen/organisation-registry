namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

using System.Linq;

public static class WithPaginationExtension
{
    public static PagedQueryable<T> WithPagination<T>(this IQueryable<T> source, IPaginationRequest paginationRequest)
        => paginationRequest.Paginate(source);
}
