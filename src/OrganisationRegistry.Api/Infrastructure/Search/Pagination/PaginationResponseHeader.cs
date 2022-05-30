namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

public class PaginationResponseHeader
{
    public int CurrentPage { get; }

    public int ItemsPerPage { get; }

    public int TotalItems { get; }

    public int TotalPages { get; }

    public PaginationResponseHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
        CurrentPage = currentPage;
        ItemsPerPage = itemsPerPage;
        TotalItems = totalItems;
        TotalPages = totalPages;
    }
}