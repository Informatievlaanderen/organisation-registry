namespace OrganisationRegistry.Api.IntegrationTests.Wiremock;

using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;

public static class WiremockSetup
{
    public static async Task Run(string wiremockUri)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(wiremockUri)
        };

        var assembly = typeof(WiremockSetup).Assembly;

        var mappingsResult = await httpClient.GetAsync("__admin/mappings");
        if (mappingsResult.IsSuccessStatusCode && (await mappingsResult.Content.ReadFromJsonAsync<Mappings>())!.Meta.Total > 0)
            return; // if there's any mapping available, we assume wiremock has been already set up.

        await AddFiles(assembly, httpClient);
        await AddMappings(assembly, httpClient);
    }

    private static async Task AddMappings(Assembly assembly, HttpClient httpClient)
    {
        foreach (var mappingName in assembly.GetManifestResourceNames().Where(s => s.StartsWith("OrganisationRegistry.Api.IntegrationTests.Wiremock.mappings")))
        {
            var mapping = assembly.GetResourceString(mappingName);

            var result = await httpClient.PostAsync(
                "__admin/mappings",
                new StringContent(mapping, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
                throw new Exception("Application setup failed: could not create wiremock mappings");
        }
    }

    private static async Task AddFiles(Assembly assembly, HttpClient httpClient)
    {
        foreach (var mappingName in assembly.GetManifestResourceNames().Where(s => s.StartsWith("OrganisationRegistry.Api.IntegrationTests.Wiremock.files")))
        {
            var mapping = assembly.GetResourceString(mappingName);

            var fileName = mappingName[mappingName.LastIndexOf(".files.", StringComparison.Ordinal)..];

            var result = await httpClient.PutAsync(
                $"__admin/files/{fileName}",
                new StringContent(mapping, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
                throw new Exception("Application setup failed: could not create wiremock mappings");
        }
    }

    private class Mappings
    {
        public MappingsMeta Meta { get; set; }

        internal class MappingsMeta
        {
            public int Total { get; set; }
        }
    }
}
