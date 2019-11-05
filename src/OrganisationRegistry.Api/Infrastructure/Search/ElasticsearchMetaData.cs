namespace OrganisationRegistry.Api.Infrastructure.Search
{
    using Nest;
    using Newtonsoft.Json;

    public class ElasticsearchMetaData<T> where T : class
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string ScrollId { get; }

        public long TimeInMs { get; }
        public long TotalItems { get; }

        public ElasticsearchMetaData(ISearchResponse<T> searchResults)
        {
            if (!string.IsNullOrWhiteSpace(searchResults.ScrollId))
                ScrollId = searchResults.ScrollId;

            TimeInMs = searchResults.Took;
            TotalItems = searchResults.Total;
        }
    }
}
