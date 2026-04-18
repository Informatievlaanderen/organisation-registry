namespace OrganisationRegistry.Api.IntegrationTests.Wiremock;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tests.Shared;

public static class WiremockSetup
{
    private const string WiremockMappingsPrefix = "OrganisationRegistry.Api.IntegrationTests.Wiremock.mappings.";
    private const string WiremockFilesPrefix = "OrganisationRegistry.Api.IntegrationTests.Wiremock.files.";
    private const string IntrospectionPath = "/connect/introspect";

    public static async Task Run(string wiremockUri)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(wiremockUri),
        };

        var assembly = typeof(WiremockSetup).Assembly;

        var mappingsResult = await httpClient.GetAsync("__admin/mappings");
        if (mappingsResult.IsSuccessStatusCode && (await mappingsResult.Content.ReadFromJsonAsync<Mappings>())!.Meta.Total > 0)
            return; // if there's any mapping available, we assume wiremock has been already set up.

        await AddFiles(assembly, httpClient);
        await AddMappings(assembly, httpClient);
    }

    public static async Task ConfigureEditApiAuthentication(string wiremockUri)
    {
        var httpClient = new HttpClient
        {
            BaseAddress = new Uri(wiremockUri),
        };

        await UpsertIntrospectionMapping(
            httpClient,
            "edit-api-introspection-cjm",
            "token-cjmClient-dv_organisatieregister_cjmbeheerder",
            "cjmClient",
            "dv_organisatieregister_cjmbeheerder");

        await UpsertIntrospectionMapping(
            httpClient,
            "edit-api-introspection-orafin",
            "token-orafinClient-dv_organisatieregister_orafinbeheerder",
            "orafinClient",
            "dv_organisatieregister_orafinbeheerder");

        await UpsertIntrospectionMapping(
            httpClient,
            "edit-api-introspection-test",
            "token-testClient-dv_organisatieregister_testclient",
            "testClient",
            "dv_organisatieregister_testclient");

        await UpsertInactiveIntrospectionMapping(httpClient);
    }

    private static async Task AddMappings(Assembly assembly, HttpClient httpClient)
    {
        foreach (var mappingName in assembly.GetManifestResourceNames().Where(s => s.StartsWith(WiremockMappingsPrefix)))
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
        foreach (var mappingName in assembly.GetManifestResourceNames().Where(s => s.StartsWith(WiremockFilesPrefix)))
        {
            var mapping = assembly.GetResourceString(mappingName);

            var fileName = mappingName.Replace(WiremockFilesPrefix, "");

            var result = await httpClient.PutAsync(
                $"__admin/files/{fileName}",
                new StringContent(mapping, Encoding.UTF8, "application/json"));

            if (!result.IsSuccessStatusCode)
                throw new Exception("Application setup failed: could not create wiremock mappings");
        }
    }

    private static async Task UpsertIntrospectionMapping(HttpClient httpClient, string id, string token, string clientId, string scope)
    {
        var response = await httpClient.PostAsJsonAsync(
            "__admin/mappings",
            new
            {
                priority = 1,
                request = new
                {
                    method = "POST",
                    urlPath = IntrospectionPath,
                    bodyPatterns = new object[]
                    {
                        new { contains = $"token={token}" },
                    },
                },
                response = new
                {
                    status = 200,
                    headers = JsonHeaders(),
                    jsonBody = new
                    {
                        active = true,
                        client_id = clientId,
                        scope,
                        dv_organisatieregister_orgcode = "OVO000001",
                    },
                },
            });

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Application setup failed: could not configure wiremock mapping '{id}'");
    }

    private static async Task UpsertInactiveIntrospectionMapping(HttpClient httpClient)
    {
        const string id = "edit-api-introspection-default";

        var response = await httpClient.PostAsJsonAsync(
            "__admin/mappings",
            new
            {
                priority = 10,
                request = new
                {
                    method = "POST",
                    urlPath = IntrospectionPath,
                },
                response = new
                {
                    status = 200,
                    headers = JsonHeaders(),
                    jsonBody = new
                    {
                        active = false,
                    },
                },
            });

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Application setup failed: could not configure wiremock mapping '{id}'");
    }

    private static Dictionary<string, string> JsonHeaders()
        => new()
        {
            ["Content-Type"] = "application/json",
        };

    private class Mappings
    {
        public MappingsMeta Meta { get; set; } = null!;

        internal class MappingsMeta
        {
            public int Total { get; set; }
        }
    }
}
