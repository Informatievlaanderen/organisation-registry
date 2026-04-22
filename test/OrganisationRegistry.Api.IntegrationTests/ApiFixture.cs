namespace OrganisationRegistry.Api.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using BackOffice;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityModel;
using IdentityModel.Client;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OrganisationRegistry.Api.Security;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Api.IntegrationTests.Wiremock;
using Xunit;

public class ApiFixture : IDisposable, IAsyncLifetime
{
    private const string DefaultApiEndpoint = "http://api.localhost:9080";
    private const string DefaultKeycloakAuthority = "http://keycloak.localhost:9080/realms/wegwijs";
    private const string ApiBaseUrlConfigurationKey = "ApiIntegrationTests:ApiBaseUrl";
    private static readonly string[] DateResponseKeys = { "date", "validFrom", "validTo" };
    private static readonly Guid ImportedParentOrganisationId = Guid.Parse("4e83f3ff-4154-4719-833c-d1a8c77568c0");
    private static readonly Guid ImportedChildOrganisationId = Guid.Parse("24fe3a2f-f5d0-4895-acac-3b1918ca1ec7");
    private static readonly TimeSpan ImportReadinessTimeout = TimeSpan.FromMinutes(3);
    private static readonly TimeSpan ReadinessPollInterval = TimeSpan.FromSeconds(2);

    public struct Orafin
    {
        public const string Client = "orafinClient";
        public const string Scope = AcmIdmConstants.Scopes.OrafinBeheerder;
    }

    public struct CJM
    {
        public const string Client = "cjmClient";
        public const string Scope = AcmIdmConstants.Scopes.CjmBeheerder;
    }

    public struct Test
    {
        public const string Client = "testClient";
        public const string Scope = AcmIdmConstants.Scopes.TestClient;
    }

    private readonly IConfigurationRoot _configurationRoot;
    private readonly OpenIdConnectConfigurationSection _openIdConnectConfiguration;

    public IOrganisationRegistryConfiguration Configuration { get; }
    public string ApiEndpoint { get; }
    public string Jwt { get; }

    public HttpClient HttpClient { get; }

    public Fixture Fixture { get; } = new();

    public ApiFixture()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        Directory.SetCurrentDirectory(rootDirectory);

        var maybeConfigurationBasePath = Directory
            .GetParent(GetType().GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeConfigurationBasePath is not { } configurationBasePath)
            throw new NullReferenceException("Configuration base path cannot be null");

        _configurationRoot = new ConfigurationBuilder()
            .SetBasePath(configurationBasePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        ApiEndpoint = _configurationRoot[ApiBaseUrlConfigurationKey] ?? DefaultApiEndpoint;
        _openIdConnectConfiguration = _configurationRoot.GetSection(OpenIdConnectConfigurationSection.Name).Get<OpenIdConnectConfigurationSection>()
            ?? throw new InvalidOperationException($"Missing '{OpenIdConnectConfigurationSection.Name}' configuration.");
        Jwt = CreateBackofficeJwt(_openIdConnectConfiguration);
        HttpClient = CreateApiClient(Jwt);
        Configuration = CreateOrganisationRegistryConfiguration(_configurationRoot);
    }

    public CreationHelpers Create
        => new(this);

    public dynamic CreateInstanceOf(Type requestType)
        => new SpecimenContext(Fixture).Resolve(requestType);

    public async Task InitializeAsync()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        await EnsureTiltApiIsAvailable();
        await ConfigureExternalDependencies();
        await EnsureParameterExists("locationtypes", Configuration.Kbo.KboV2RegisteredOfficeLocationTypeId, "KBO Location");
        await EnsureParameterExists("organisationclassificationtypes", Configuration.Kbo.KboV2LegalFormOrganisationClassificationTypeId, "KBO Classification");
        await EnsureParameterExists("organisationclassificationtypes", Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.First(), "CJM Classification 1");
        await EnsureParameterExists("organisationclassificationtypes", Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.Last(), "CJM Classification 2");
        await EnsureParameterExists("labeltypes", Configuration.Kbo.KboV2FormalNameLabelTypeId, "KBO label");
        await EnsureParameterExists("keytypes", Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First(), "orafin key type");
        await EnsureImportedDataIsReady();
    }

    public async Task<HttpClient> CreateCjmClient()
        => await CreateMachine2MachineClientFor(CJM.Client, CJM.Scope);

    public async Task<HttpClient> CreateOrafinClient()
        => await CreateMachine2MachineClientFor(Orafin.Client, Orafin.Scope);

    public async Task<HttpClient> CreateTestClient()
        => await CreateMachine2MachineClientFor(Test.Client, Test.Scope);

    public async Task<HttpClient> CreateMachine2MachineClientFor(string clientId, string scope)
    {
        var httpClientFor = CreateApiClient(await GetMachineToMachineToken(clientId, scope));
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return httpClientFor;
    }

    public static async Task<HttpResponseMessage> Post(HttpClient httpClient, string route, object body)
        => await httpClient.PostAsync(route, ToJson(body));

    public static async Task<HttpResponseMessage> Put(HttpClient httpClient, string route, object body)
        => await httpClient.PutAsync(route, ToJson(body));

    public static async Task<HttpResponseMessage> Get(HttpClient httpClient, string route)
        => await httpClient.GetAsync(route);

    public static async Task<HttpResponseMessage> Delete(HttpClient httpClient, string route)
        => await httpClient.DeleteAsync(route);

    private HttpClient CreateApiClient(string token)
        => new()
        {
            BaseAddress = new Uri(ApiEndpoint),
            DefaultRequestHeaders =
            {
                Authorization = new AuthenticationHeaderValue("Bearer", token),
            },
        };

    private static IOrganisationRegistryConfiguration CreateOrganisationRegistryConfiguration(IConfigurationRoot configurationRoot)
    {
        var apiConfiguration = configurationRoot.GetSection(ApiConfigurationSection.Name).Get<ApiConfigurationSection>()
            ?? throw new InvalidOperationException($"Missing '{ApiConfigurationSection.Name}' configuration.");

        return new OrganisationRegistryConfiguration(
            apiConfiguration,
            configurationRoot.GetSection(OrganisationTerminationConfigurationSection.Name).Get<OrganisationTerminationConfigurationSection>(),
            configurationRoot.GetSection(AuthorizationConfigurationSection.Name).Get<AuthorizationConfigurationSection>(),
            configurationRoot.GetSection(CachingConfigurationSection.Name).Get<CachingConfigurationSection>(),
            configurationRoot.GetSection(HostedServicesConfigurationSection.Name).Get<HostedServicesConfigurationSection>());
    }

    private static string CreateBackofficeJwt(OpenIdConnectConfigurationSection openIdConnectConfiguration)
    {
        var identity = new ClaimsIdentity();
        identity.AddClaim(new Claim(JwtClaimTypes.Subject, "api-integration-tests"));
        identity.AddClaim(new Claim(JwtClaimTypes.GivenName, "Algemeenbeheerder"));
        identity.AddClaim(new Claim(JwtClaimTypes.FamilyName, "Persona"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.AcmId, "A5C5BFCD-266C-4CC7-9869-4B95E34C090D"));
        identity.AddClaim(new Claim(AcmIdmConstants.Claims.Role, "WegwijsBeheerder-algemeenbeheerder:OVO002949"));

        var tokenBuilder = new OrganisationRegistryTokenBuilder(openIdConnectConfiguration);
        return tokenBuilder.BuildJwt(tokenBuilder.ParseRoles(identity));
    }

    private async Task ConfigureExternalDependencies()
    {
        var wiremockEndpoint = _configurationRoot["Api:KboMagdaEndpoint"];
        if (string.IsNullOrWhiteSpace(wiremockEndpoint))
            return;

        await WiremockSetup.Run(wiremockEndpoint);
        await WiremockSetup.ConfigureEditApiAuthentication(wiremockEndpoint);
    }

    private async Task EnsureImportedDataIsReady()
    {
        if (!await HasImportedOrganisationsAsync())
            OrganisationRegistry.Import.Piavo.Program.Import(ApiEndpoint, Jwt);

        await WaitUntilAsync(
            HasImportedOrganisationsAsync,
            ImportReadinessTimeout,
            "Tilt importdata is niet klaar. " +
            "Controleer of de Piavo-import gelopen heeft en of de projecties afgewerkt zijn.");

        await EnsureImportedOrganisationCoverage();

        await WaitUntilAsync(
            HasImportedReadModelCoverageAsync,
            ImportReadinessTimeout,
            "Tilt importdata is onvolledig voor de integration tests. " +
            "Controleer of de read models afgewerkt zijn.");
    }

    private async Task EnsureParameterExists(string requestUri, Guid id, string name)
    {
        using var existingResponse = await Get(HttpClient, $"/v1/{requestUri}/{id}");
        if (existingResponse.StatusCode != HttpStatusCode.NotFound)
            return;

        using var createResponse = await Post(HttpClient, $"/v1/{requestUri}", new { id, name });
        if (createResponse.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK)
            return;

        throw new InvalidOperationException(
            $"Could not ensure parameter '{requestUri}/{id}'. " +
            $"Status: {createResponse.StatusCode}. Body: {await createResponse.Content.ReadAsStringAsync()}");
    }

    private async Task EnsureTiltApiIsAvailable()
    {
        try
        {
            using var response = await HttpClient.GetAsync("/v1/status");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception exception)
        {
            throw new InvalidOperationException(
                $"API integration tests verwachten een draaiende Tilt-omgeving op '{ApiEndpoint}'. " +
                "Start Tilt eerst, bijvoorbeeld met 'tilt up'.",
                exception);
        }
    }

    private async Task<bool> HasImportedOrganisationsAsync()
    {
        return await RouteExists($"/v1/organisations/{ImportedParentOrganisationId}") &&
               await RouteExists($"/v1/organisations/{ImportedChildOrganisationId}");
    }

    private async Task<bool> HasImportedReadModelCoverageAsync()
    {
        if (!await HasAnyItems("/v1/people") ||
            !await HasAnyItems("/v1/buildings") ||
            !await HasAnyItems("/v1/functiontypes") ||
            !await HasAnyItems("/v1/capacities"))
            return false;

        return await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/contacts") &&
               await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/keys") &&
               await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/capacities") &&
               await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/children") &&
               await HasAnyItems($"/v1/organisations/{ImportedChildOrganisationId}/classifications");
    }

    private async Task<bool> HasAnyItems(string route)
    {
        using var response = await Get(HttpClient, route);
        if (!response.IsSuccessStatusCode)
            return false;

        var items = await DeserializeAsList(response);
        return items.Length > 0;
    }

    private async Task<bool> RouteExists(string route)
    {
        using var response = await Get(HttpClient, route);
        return response.IsSuccessStatusCode;
    }

    private async Task EnsureImportedOrganisationCoverage()
    {
        await EnsureImportedOrganisationHasKey();
        await EnsureImportedOrganisationHasCapacity();
        await EnsureImportedOrganisationHasClassification();
    }

    private async Task EnsureImportedOrganisationHasKey()
    {
        if (await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/keys"))
            return;

        using var response = await Post(
            HttpClient,
            $"/v1/organisations/{ImportedParentOrganisationId}/keys",
            new
            {
                OrganisationKeyId = Guid.NewGuid(),
                KeyTypeId = Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First(),
                KeyValue = $"TILT-READY-{ImportedParentOrganisationId:N}",
                ValidFrom = (DateTime?)null,
                ValidTo = (DateTime?)null,
            });

        await VerifyStatusCode(response, HttpStatusCode.Created);
    }

    private async Task EnsureImportedOrganisationHasCapacity()
    {
        if (await HasAnyItems($"/v1/organisations/{ImportedParentOrganisationId}/capacities"))
            return;

        var personId = await Create.Person();
        var functionId = await Create.Function();
        var capacityId = await Create.Capacity();

        using var response = await Post(
            HttpClient,
            $"/v1/organisations/{ImportedParentOrganisationId}/capacities",
            new
            {
                OrganisationCapacityId = Guid.NewGuid(),
                CapacityId = capacityId,
                PersonId = personId,
                FunctionId = functionId,
                LocationId = (Guid?)null,
                Contacts = new Dictionary<Guid, string>(),
                ValidFrom = (DateTime?)null,
                ValidTo = (DateTime?)null,
            });

        await VerifyStatusCode(response, HttpStatusCode.Created);
    }

    private async Task EnsureImportedOrganisationHasClassification()
    {
        if (await HasAnyItems($"/v1/organisations/{ImportedChildOrganisationId}/classifications"))
            return;

        var organisationClassificationTypeId = Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.First();
        var organisationClassificationId = await Create.OrganisationClassification(organisationClassificationTypeId);

        using var response = await Post(
            HttpClient,
            $"/v1/organisations/{ImportedChildOrganisationId}/classifications",
            new
            {
                OrganisationOrganisationClassificationId = Guid.NewGuid(),
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationClassificationId = organisationClassificationId,
                ValidFrom = (DateTime?)null,
                ValidTo = (DateTime?)null,
            });

        await VerifyStatusCode(response, HttpStatusCode.Created);
    }

    private async Task<string> GetMachineToMachineToken(string clientId, string scope)
    {
        var editApiConfiguration = _configurationRoot.GetSection(EditApiConfigurationSection.Name)
            .Get<EditApiConfigurationSection>();

        var authority = string.IsNullOrWhiteSpace(editApiConfiguration?.Authority)
            ? DefaultKeycloakAuthority
            : editApiConfiguration.Authority;

        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{authority.TrimEnd('/')}/protocol/openid-connect/token",
                ClientId = clientId,
                ClientSecret = "secret",
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var response = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);

        if (response.IsError || string.IsNullOrWhiteSpace(response.AccessToken))
            throw new InvalidOperationException(
                $"Could not retrieve Keycloak M2M token for '{clientId}' from '{authority}'. " +
                $"Error: {response.Error}. Description: {response.ErrorDescription}.");

        return response.AccessToken;
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> predicate, TimeSpan timeout, string timeoutMessage)
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        Exception? lastException = null;

        while (DateTime.UtcNow < deadline)
        {
            try
            {
                if (await predicate())
                    return;

                lastException = null;
            }
            catch (Exception exception)
            {
                lastException = exception;
            }

            await Task.Delay(ReadinessPollInterval);
        }

        throw new InvalidOperationException(timeoutMessage, lastException);
    }

    private static StringContent ToJson(object body)
        => new(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");

    public static async Task VerifyStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        if (response.StatusCode != expectedStatusCode)
            throw new AssertionFailedException(
                $"Expected statuscode {expectedStatusCode}, but received {response.StatusCode}.\n" +
                $"The response was '{await response.Content.ReadAsStringAsync()}'\n");
    }

    public static async Task<Dictionary<string, object>> ValidateResponse(Type responseType, HttpResponseMessage actualResponseMessage, dynamic expectedResponse)
    {
        var deserializedResponse = await Deserialize(actualResponseMessage);
        deserializedResponse.Should().NotBeNull();

        var properties = responseType.GetProperties();
        foreach (var propertyInfo in properties)
        {
            var propertyKey = propertyInfo.Name.ToLowerInvariant();

            if (!deserializedResponse.ContainsKey(propertyKey))
                continue;

            var expectedValue = propertyInfo.GetValue(expectedResponse, null);

            deserializedResponse[propertyKey].ToString().Should().Be(expectedValue.ToString());
        }

        return deserializedResponse;
    }

    public async Task RemoveAndVerify(string baseRoute, Guid id)
    {
        var deleteResponse = await Delete(HttpClient, $"{baseRoute}/{id}");
        await VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        var getResponse = await Get(HttpClient, $"{baseRoute}/{id}");
        await VerifyStatusCode(getResponse, HttpStatusCode.NotFound);
    }

    public async Task GetListAndVerify(string route)
    {
        var getResponse = await Get(HttpClient, $"{route}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var deserializedResponse = await DeserializeAsList(getResponse);
        deserializedResponse.Should().NotBeNull();

        deserializedResponse.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    public async Task CreateWithInvalidDataAndVerifyBadRequest(string route)
    {
        var createResponse = await Post(HttpClient, $"{route}", "prut");
        await VerifyStatusCode(createResponse, HttpStatusCode.BadRequest);
    }

    public async Task UpdateWithInvalidDataAndVerifyBadRequest(string baseRoute, Guid id)
    {
        var updateResponse = await Put(HttpClient, $"{baseRoute}/{id}", "prut");
        await VerifyStatusCode(updateResponse, HttpStatusCode.BadRequest);
    }

    public async Task CreateAndVerify<T>(string baseRoute, T body, Action<Dictionary<string, object>, T> verifyResult)
        where T : notnull
    {
        var createResponse = await Post(
            HttpClient,
            baseRoute,
            body);
        await VerifyStatusCode(createResponse, HttpStatusCode.Created);

        var getResponse = await Get(HttpClient, createResponse.Headers.Location!.ToString());
        await VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await Deserialize(getResponse);
        verifyResult(responseBody, body);
    }

    public async Task UpdateAndVerify<T>(string baseRoute, Guid id, T body, Action<Dictionary<string, object>, T> verifyResult)
        where T : notnull
    {
        var updateResponse = await Put(
            HttpClient,
            $"{baseRoute}/{id}",
            body);
        await VerifyStatusCode(updateResponse, HttpStatusCode.OK);

        var getResponse = await Get(HttpClient, $"{baseRoute}/{id}");
        await VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await Deserialize(getResponse);
        verifyResult(responseBody, body);
    }

    public static Guid GetIdFrom(HttpResponseHeaders headers)
        => new(headers.Location!.ToString().Split('/').Last());

    public static async Task<Dictionary<string, object>> Deserialize(HttpResponseMessage message)
        => NormalizeDateValues(JsonConvert.DeserializeObject<Dictionary<string, object>>(await message.Content.ReadAsStringAsync())!);

    public static async Task<Dictionary<string, object>[]> DeserializeAsList(HttpResponseMessage message)
        => JsonConvert.DeserializeObject<Dictionary<string, object>[]>(await message.Content.ReadAsStringAsync())!
            .Select(NormalizeDateValues)
            .ToArray();

    private static Dictionary<string, object> NormalizeDateValues(Dictionary<string, object> responseBody)
    {
        foreach (var key in DateResponseKeys)
        {
            if (!responseBody.TryGetValue(key, out var value) || value is null)
                continue;

            if (value is DateTime dateTime)
            {
                responseBody[key] = dateTime.Date;
                continue;
            }

            if (DateTime.TryParse(value.ToString(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var parsedDate))
                responseBody[key] = parsedDate.Date;
        }

        return responseBody;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            HttpClient.Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
