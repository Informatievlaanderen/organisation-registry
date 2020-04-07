namespace OrganisationRegistry.Infrastructure
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Configuration;
    using Microsoft.Extensions.Options;

    public interface IExternalIpFetcher
    {
        Task<string> Fetch();
    }

    public class ExternalIpFetcher : IExternalIpFetcher
    {
        private readonly InfrastructureConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ExternalIpFetcher(
            IOptions<InfrastructureConfiguration> configuration,
            IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration.Value;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Fetch()
        {
            var httpClient = _httpClientFactory.CreateClient();
            httpClient.BaseAddress = new Uri(_configuration.ExternalIpServiceUri);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var httpResponseMessage = await httpClient.GetAsync("");
            return await httpResponseMessage.Content.ReadAsStringAsync();
        }
    }
}
