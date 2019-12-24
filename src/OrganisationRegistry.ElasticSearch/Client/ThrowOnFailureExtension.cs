namespace OrganisationRegistry.ElasticSearch.Client
{
    using Nest;

    public static class ThrowOnFailureExtension
    {
        public static void ThrowOnFailure(this IndexResponse response)
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);
        }

        public static void ThrowOnFailure(this UpdateByQueryResponse response)
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

        public static void ThrowOnFailure(this BulkResponse response)
        {
            if (!response.IsValid)
                throw new ElasticsearchException(response.DebugInformation);
        }
    }
}
