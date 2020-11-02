namespace OrganisationRegistry.Api.Infrastructure.Search
{
    public class SearchConstants
    {
        public const string SearchMetaDataHeaderName = "x-search-metadata";

        // TODO: Lazy load from database and cache
        // TODO: Expose these type of settings over a json endpoint as well
        public int PageSize => 10;
    }
}
