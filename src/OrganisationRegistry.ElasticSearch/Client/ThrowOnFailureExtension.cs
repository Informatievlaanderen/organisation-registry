namespace OrganisationRegistry.ElasticSearch.Client
{
    using Nest;

    public static class ThrowOnFailureExtension
    {
        public static void ThrowOnFailure(this IIndexResponse response)
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);
        }

        public static void ThrowOnFailure(this IUpdateByQueryResponse response)
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);
        }

        public static IGetResponse<T> ThrowOnFailure<T>(this IGetResponse<T> response) where T : class
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);

            return response;
        }

        public static ISearchResponse<T> ThrowOnFailure<T>(this ISearchResponse<T> response) where T : class
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);

            return response;
        }

        public static void ThrowOnFailure(this IBulkResponse response)
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);
        }
    }
}
