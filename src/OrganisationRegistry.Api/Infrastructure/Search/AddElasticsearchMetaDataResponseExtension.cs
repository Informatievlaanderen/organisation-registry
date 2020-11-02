namespace OrganisationRegistry.Api.Infrastructure.Search
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class AddElasticsearchMetaDataResponseExtension
    {
        public static void AddElasticsearchMetaDataResponse<T>(this HttpResponse response, ElasticsearchMetaData<T> metaData) where T : class
        {
            response.Headers.Add(SearchConstants.SearchMetaDataHeaderName, JsonConvert.SerializeObject(metaData));
        }
    }
}
