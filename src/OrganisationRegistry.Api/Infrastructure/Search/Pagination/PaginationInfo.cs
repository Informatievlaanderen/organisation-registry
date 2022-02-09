namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination
{
    public class PaginationInfo
    {
        public int CurrentPage { get; }

        public int ItemsPerPage { get; }

        public int TotalItems { get; }

        public int TotalPages { get; }

        public PaginationInfo(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemsPerPage = itemsPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }
    }
}
