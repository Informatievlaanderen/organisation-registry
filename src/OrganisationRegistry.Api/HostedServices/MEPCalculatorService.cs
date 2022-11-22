namespace OrganisationRegistry.Api.HostedServices;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using Microsoft.Extensions.Logging;

public class MEPCalculatorService : BackgroundService
{
    private readonly Elastic _elastic;
    private readonly IHttpClientFactory _httpClientFactory;

    public MEPCalculatorService(
        ILogger logger,
        Elastic elastic,
        IHttpClientFactory httpClientFactory) : base(logger)
    {
        _elastic = elastic;
        _httpClientFactory = httpClientFactory;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        // 1 Get All Bodies
        var bodies = await _elastic.ReadClient.SearchAsync<BodyDocument>(ct: cancellationToken);

        using var client = _httpClientFactory.CreateClient();
        // 2 Foreach Body
        foreach (var body in bodies.Documents)
        {
            // 2.1 Get MEP
            client.GetAsync($"/v1/reports/bodyparticipation/{body.Id}");

            // 2.2 Save MEP bij Body
        }

        // 3 Rebuild index
    }
}
