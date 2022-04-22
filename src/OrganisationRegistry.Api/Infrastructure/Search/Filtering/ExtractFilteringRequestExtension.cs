namespace OrganisationRegistry.Api.Infrastructure.Search.Filtering
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public static class ExtractFilteringRequestExtension
    {
        public static FilteringHeader<T> ExtractFilteringRequest<T>(this HttpRequest request) where T: new()
        {
            var filtering = request.Headers["x-filtering"];

            if (string.IsNullOrEmpty(filtering))
                return new FilteringHeader<T>(default);

            try
            {
                return new FilteringHeader<T>(JsonConvert.DeserializeObject<T>(Uri.UnescapeDataString(filtering)));
            }
            catch
            {
                return new FilteringHeader<T>(default);
            }
        }
    }
}
