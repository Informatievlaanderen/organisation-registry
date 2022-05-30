namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

public class PaginationRequestHeader
{
    public int RequestedPage { get; }

    public int ItemsPerPage { get; }

    public PaginationRequestHeader(int requestedPage, int itemsPerPage)
    {
        RequestedPage = requestedPage;
        ItemsPerPage = itemsPerPage;
    }
}