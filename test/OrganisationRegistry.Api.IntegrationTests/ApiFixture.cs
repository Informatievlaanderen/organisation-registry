namespace OrganisationRegistry.Api.IntegrationTests;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.Kernel;
using Backoffice.Organisation.OrganisationClassification;
using Backoffice.Parameters.FormalFramework.Requests;
using Backoffice.Parameters.FormalFrameworkCategory.Requests;
using Backoffice.Parameters.FunctionType.Requests;
using Backoffice.Person.Detail;
using FluentAssertions;
using FluentAssertions.Execution;
using IdentityModel;
using IdentityModel.Client;
using Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Person;
using SqlServer.Configuration;
using SqlServer.Infrastructure;
using Wiremock;
using Xunit;
using CreateOrganisationClassificationRequest = Backoffice.Parameters.OrganisationClassification.Requests.CreateOrganisationClassificationRequest;
using CreateOrganisationClassificationTypeRequest = Backoffice.Parameters.OrganisationClassificationType.Requests.CreateOrganisationClassificationTypeRequest;

public class ApiFixture : IDisposable, IAsyncLifetime
{
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

    private readonly IWebHost _webHost;
    private readonly IConfigurationRoot? _configurationRoot;
    public IOrganisationRegistryConfiguration Configuration { get; }
    public const string ApiEndpoint = "http://localhost:5000";
    public const string Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdF9oYXNoIjoiMklEMHdGR3l6WnJWaHRmbi00Ty1EQSIsImF1ZCI6WyJodHRwczovL2RpZW5zdHZlcmxlbmluZy10ZXN0LmJhc2lzcmVnaXN0ZXJzLnZsYWFuZGVyZW4iXSwiYXpwIjoiN2Q4MDExOTctNmQ0My00NzZhLTgzZWYtMzU4NjllZTUyZDg1IiwiZXhwIjoxODkzOTM2ODIzLCJmYW1pbHlfbmFtZSI6IkFwaSIsImdpdmVuX25hbWUiOiJUZXN0IiwiaWF0IjoxNTc4MzExNjMzLCJ2b19pZCI6IjEyMzk4Nzk4Ny0xMjMxMjMiLCJpc3MiOiJodHRwczovL2RpZW5zdHZlcmxlbmluZy10ZXN0LmJhc2lzcmVnaXN0ZXJzLnZsYWFuZGVyZW4iLCJ1cm46YmU6dmxhYW5kZXJlbjpkaWVuc3R2ZXJsZW5pbmc6YWNtaWQiOiJ2b19pZCIsInVybjpiZTp2bGFhbmRlcmVuOmFjbTpmYW1pbGllbmFhbSI6ImZhbWlseV9uYW1lIiwidXJuOmJlOnZsYWFuZGVyZW46YWNtOnZvb3JuYWFtIjoiZ2l2ZW5fbmFtZSIsInVybjpiZTp2bGFhbmRlcmVuOndlZ3dpanM6YWNtaWQiOiJ0ZXN0Iiwicm9sZSI6WyJhbGdlbWVlbkJlaGVlcmRlciJdLCJuYmYiOjE1NzgzOTY2MzN9.wWYDfwbcBxHMdaBIhoFH0UnXNl82lE_rsu-R49km1FM";

    public HttpClient HttpClient { get; } = new()
    {
        BaseAddress = new Uri(ApiEndpoint),
        DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", Jwt) },
    };

    public Fixture Fixture { get; } = new();

    public ApiFixture()
    {
        var maybeRootDirectory = Directory
            .GetParent(typeof(Startup).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;
        if (maybeRootDirectory is not { } rootDirectory)
            throw new NullReferenceException("Root directory cannot be null");

        Directory.SetCurrentDirectory(rootDirectory);

        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true);

        var connectionString =
            builder.Build()
                .GetSection(SqlServerConfiguration.Section)
                .Get<SqlServerConfiguration>()
                .MigrationsConnectionString;

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseSqlServer(
                connectionString,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema))
            .Options;

        new OrganisationRegistryContext(dbContextOptions).Database.EnsureDeleted();

        IWebHostBuilder hostBuilder = new WebHostBuilder();
        var environment = hostBuilder.GetSetting("environment");

        if (environment == "Development")
        {
            var cert = new X509Certificate2("organisationregistry-api.pfx", "organisationregistry");

            hostBuilder = hostBuilder
                .UseKestrel(
                    server =>
                    {
                        server.AddServerHeader = false;
                        server.Listen(IPAddress.Any, 2443, listenOptions => listenOptions.UseHttps(cert));
                    })
                .UseUrls("https://api.organisatie.dev-vlaanderen.local:2443");
        }
        else
        {
            hostBuilder = hostBuilder.UseKestrel(server => server.AddServerHeader = false);
        }

        _configurationRoot = builder.Build();

        WiremockSetup.Run(_configurationRoot["Api:KboMagdaEndpoint"]).GetAwaiter().GetResult();

        _webHost = hostBuilder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(_configurationRoot)
            .UseStartup<Startup>()
            .Build();

        _webHost.Start();

        Configuration = _webHost.Services.GetRequiredService<IOrganisationRegistryConfiguration>();
    }


    public dynamic CreateInstanceOf(Type requestType)
        => new SpecimenContext(Fixture).Resolve(requestType);

    public async Task InitializeAsync()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        OrganisationRegistry.Import.Piavo.Program.Import(
            ApiEndpoint,
            Jwt);

        await CreateParameter("locationtypes", Configuration.Kbo.KboV2RegisteredOfficeLocationTypeId, "KBO Location");
        await CreateParameter("organisationclassificationtypes", Configuration.Kbo.KboV2LegalFormOrganisationClassificationTypeId, "KBO Classification");
        await CreateParameter("organisationclassificationtypes", Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.First(), "CJM Classification 1");
        await CreateParameter("organisationclassificationtypes", Configuration.Authorization.OrganisationClassificationTypeIdsOwnedByCjm.Last(), "CJM Classification 2");
        await CreateParameter("labeltypes", Configuration.Kbo.KboV2FormalNameLabelTypeId, "KBO label");
        await CreateParameter("keytypes", Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First(), "orafin key type");
    }

    private Task CreateParameter(string requestUri, Guid id, string name)
        => Post(HttpClient, $"/v1/{requestUri}", new { id = id, name = name });

    public async Task<HttpClient> CreateCjmClient()
        => await CreateMachine2MachineClientFor(CJM.Client, CJM.Scope);

    public async Task<HttpClient> CreateOrafinClient()
        => await CreateMachine2MachineClientFor(Orafin.Client, Orafin.Scope);

    public async Task<HttpClient> CreateTestClient()
        => await CreateMachine2MachineClientFor(Test.Client, Test.Scope);

    public async Task<HttpClient> CreateMachine2MachineClientFor(string clientId, string scope)
    {
        var editApiConfiguration = _configurationRoot!.GetSection(EditApiConfigurationSection.Name)
            .Get<EditApiConfigurationSection>();

        var tokenClient = new TokenClient(
            () => new HttpClient(),
            new TokenClientOptions
            {
                Address = $"{editApiConfiguration.Authority}/connect/token",
                ClientId = clientId,
                ClientSecret = "secret",
                Parameters = new Parameters(
                    new[]
                    {
                        new KeyValuePair<string, string>("scope", scope),
                    }),
            });

        var acmResponse = await tokenClient.RequestTokenAsync(OidcConstants.GrantTypes.ClientCredentials);
        var token = acmResponse.AccessToken;
        var httpClientFor = new HttpClient
        {
            BaseAddress = new Uri(ApiEndpoint),
        };
        httpClientFor.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        httpClientFor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return httpClientFor;
    }

    public Task CreateOrganisation(Guid organisationId, string organisationName)
        => Post(HttpClient, "/v1/organisations", new { id = organisationId, name = organisationName });

    public async Task<Guid> CreateOrganisationClassificationType(bool allowDifferentClassificationsToOverlap)
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            "/v1/organisationclassificationtypes",
            new CreateOrganisationClassificationTypeRequest
            {
                Id = id,
                Name = Fixture.Create<string>(),
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });
        return id;
    }

    public async Task<Guid> CreateOrganisationClassification(Guid organisationClassificationTypeId)
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            "/v1/organisationclassifications",
            new CreateOrganisationClassificationRequest
            {
                Id = id,
                Name = Fixture.Create<string>(),
                Order = Fixture.Create<int>(),
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                Active = true,
                ExternalKey = null,
            });
        return id;
    }

    public async Task<Guid> CreateOrganisationOrganisationClassification(Guid organisationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            $"/v1/organisation/{organisationId}/classifications",
            new AddOrganisationOrganisationClassificationRequest
            {
                OrganisationOrganisationClassificationId = id,
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationClassificationId = organisationClassificationId,
                ValidFrom = null,
                ValidTo = null,
            });
        return id;
    }

    public async Task CreateContactType(Guid contactTypeId, string? contactTypeName = null)
    {
        await Post(
            HttpClient,
            $"/v1/contacttypes",
            new { Id = contactTypeId, Name = contactTypeName ?? Fixture.Create<string>() });
    }

    public async Task<Guid> CreateFormalFramework(Guid formalFrameworkCategoryId)
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            $"/v1/formalframeworks",
            new CreateFormalFrameworkRequest
            {
                Id = id,
                Name = Fixture.Create<string>(),
                Code = Fixture.Create<string>(),
                FormalFrameworkCategoryId = formalFrameworkCategoryId,
            });
        return id;
    }

    public async Task<Guid> CreateFormalFrameworkCategory()
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            $"/v1/formalframeworkcategories",
            new CreateFormalFrameworkCategoryRequest()
            {
                Id = id,
                Name = Fixture.Create<string>(),
            });
        return id;
    }

    public async Task<Guid> CreatePerson()
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            $"/v1/people",
            new CreatePersonRequest
            {
                Id = id,
                Name = Fixture.Create<string>(),
                FirstName = Fixture.Create<string>(),
                Sex = Fixture.Create<bool>() ? Sex.Male : Sex.Female,
                DateOfBirth = Fixture.Create<DateTime>(),
            });
        return id;
    }

    /// <summary>
    /// Function or FunctionType, make up your f***ing mind
    /// </summary>
    /// <returns></returns>
    public async Task<Guid> CreateFunction()
    {
        var id = Fixture.Create<Guid>();
        await Post(
            HttpClient,
            $"/v1/functiontypes",
            new CreateFunctionTypeRequest()
            {
                Id = id,
                Name = Fixture.Create<string>(),
            });
        return id;
    }

    public async Task<HttpResponseMessage> Create(string baseRoute, object body)
        => await Post(HttpClient, $"{baseRoute}", body);

    public async Task<HttpResponseMessage> Update(string baseRoute, Guid id, object updateRequest)
        => await Put(HttpClient, $"{baseRoute}/{id}", updateRequest);

    public static async Task<HttpResponseMessage> Post(HttpClient httpClient, string route, object body)
        => await httpClient.PostAsync(route, ToJson(body));

    public static async Task<HttpResponseMessage> Put(HttpClient httpClient, string route, object body)
        => await httpClient.PutAsync(route, ToJson(body));

    public static async Task<HttpResponseMessage> Get(HttpClient httpClient, string route)
        => await httpClient.GetAsync(route);

    public static async Task<HttpResponseMessage> Delete(HttpClient httpClient, string route)
        => await httpClient.DeleteAsync(route);

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
        // remove
        var deleteResponse = await Delete(HttpClient, $"{baseRoute}/{id}");
        await VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        // get
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
        // update
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

        // get
        var getResponse = await Get(HttpClient, createResponse.Headers.Location!.ToString());
        await VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await Deserialize(getResponse);
        verifyResult(responseBody, body);
    }

    public async Task UpdateAndVerify<T>(string baseRoute, Guid id, T body, Action<Dictionary<string, object>, T> verifyResult)
        where T : notnull
    {
        // update
        var updateResponse = await Put(
            HttpClient,
            $"{baseRoute}/{id}",
            body);
        await VerifyStatusCode(updateResponse, HttpStatusCode.OK);

        // get
        var getResponse = await Get(HttpClient, $"{baseRoute}/{id}");
        await VerifyStatusCode(getResponse, HttpStatusCode.OK);

        var responseBody = await Deserialize(getResponse);
        verifyResult(responseBody, body);
    }

    public static Guid GetIdFrom(HttpResponseHeaders headers)
        => new(headers.Location!.ToString().Split('/').Last());

    public static async Task<Dictionary<string, object>> Deserialize(HttpResponseMessage message)
        => JsonConvert.DeserializeObject<Dictionary<string, object>>(await message.Content.ReadAsStringAsync())!;

    public static async Task<Dictionary<string, object>[]> DeserializeAsList(HttpResponseMessage message)
        => JsonConvert.DeserializeObject<Dictionary<string, object>[]>(await message.Content.ReadAsStringAsync())!;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
            _webHost.Dispose();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    public Task DisposeAsync()
        => Task.CompletedTask;
}
