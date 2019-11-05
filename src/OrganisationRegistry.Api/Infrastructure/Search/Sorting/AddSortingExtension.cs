namespace OrganisationRegistry.Api.Infrastructure.Search.Sorting
{
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class AddSortingExtension
    {
        public static void AddSortingResponse(this HttpResponse response, string sortBy, SortOrder sortOrder)
        {
            var sortingHeader = new SortingHeader(sortBy.ToLowerInvariant(), sortOrder);

            response.Headers.Add("x-sorting", JsonConvert.SerializeObject(sortingHeader));
        }
    }
}
