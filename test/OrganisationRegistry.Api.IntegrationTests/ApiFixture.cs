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
using SqlServer.Configuration;
using SqlServer.Infrastructure;
using Xunit;

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

    private readonly IWebHost _webHost;
    private readonly IConfigurationRoot? _configurationRoot;
    public IOrganisationRegistryConfiguration Configuration { get; }
    public const string ApiEndpoint = "http://localhost:5000/v1/";
    public const string Jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJhdF9oYXNoIjoiMklEMHdGR3l6WnJWaHRmbi00Ty1EQSIsImF1ZCI6WyJodHRwczovL2RpZW5zdHZlcmxlbmluZy10ZXN0LmJhc2lzcmVnaXN0ZXJzLnZsYWFuZGVyZW4iXSwiYXpwIjoiN2Q4MDExOTctNmQ0My00NzZhLTgzZWYtMzU4NjllZTUyZDg1IiwiZXhwIjoxODkzOTM2ODIzLCJmYW1pbHlfbmFtZSI6IkFwaSIsImdpdmVuX25hbWUiOiJUZXN0IiwiaWF0IjoxNTc4MzExNjMzLCJ2b19pZCI6IjEyMzk4Nzk4Ny0xMjMxMjMiLCJpc3MiOiJodHRwczovL2RpZW5zdHZlcmxlbmluZy10ZXN0LmJhc2lzcmVnaXN0ZXJzLnZsYWFuZGVyZW4iLCJ1cm46YmU6dmxhYW5kZXJlbjpkaWVuc3R2ZXJsZW5pbmc6YWNtaWQiOiJ2b19pZCIsInVybjpiZTp2bGFhbmRlcmVuOmFjbTpmYW1pbGllbmFhbSI6ImZhbWlseV9uYW1lIiwidXJuOmJlOnZsYWFuZGVyZW46YWNtOnZvb3JuYWFtIjoiZ2l2ZW5fbmFtZSIsInVybjpiZTp2bGFhbmRlcmVuOndlZ3dpanM6YWNtaWQiOiJ0ZXN0Iiwicm9sZSI6WyJhbGdlbWVlbkJlaGVlcmRlciJdLCJuYmYiOjE1NzgzOTY2MzN9.wWYDfwbcBxHMdaBIhoFH0UnXNl82lE_rsu-R49km1FM";


    public HttpClient HttpClient { get; } = new()
    {
        BaseAddress = new Uri(ApiEndpoint),
        DefaultRequestHeaders = { Authorization = new AuthenticationHeaderValue("Bearer", Jwt) },
    };

    public object? FixtureContext { get; set; }

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
        _webHost = hostBuilder
            .UseContentRoot(Directory.GetCurrentDirectory())
            .UseConfiguration(_configurationRoot)
            .UseStartup<Startup>()
            .Build();

        _webHost.Start();

        Configuration = _webHost.Services.GetRequiredService<IOrganisationRegistryConfiguration>();
    }

    public async Task InitializeAsync()
    {
        HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        OrganisationRegistry.Import.Piavo.Program.Import(
            ApiEndpoint,
            Jwt);

        await CreateParameter("locationtypes", Configuration.Kbo.KboV2RegisteredOfficeLocationTypeId, "KBO Location");
        await CreateParameter("organisationclassificationtypes", Configuration.Kbo.KboV2LegalFormOrganisationClassificationTypeId, "KBO Classification");
        await CreateParameter("labeltypes", Configuration.Kbo.KboV2FormalNameLabelTypeId, "KBO label");
        await CreateParameter("keytypes", Configuration.Authorization.KeyIdsAllowedOnlyForOrafin.First(), "orafin key type");
    }

    private Task CreateParameter(string requestUri, Guid id, string name)
        => Post(HttpClient, requestUri, new { id = id, name = name });

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
        => Post(HttpClient, "organisations", new { id = organisationId, name = organisationName });

    public static async Task<HttpResponseMessage> Post(HttpClient httpClient, string route, object body)
        => await httpClient.PostAsync(
            route,
            new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"));

    public static async Task<HttpResponseMessage> Put(HttpClient httpClient, string route, object body)
        => await httpClient.PutAsync(
            route,
            new StringContent(
                JsonConvert.SerializeObject(body),
                Encoding.UTF8,
                "application/json"));

    public static async Task VerifyStatusCode(HttpResponseMessage response, HttpStatusCode expectedStatusCode)
    {
        if (response.StatusCode != expectedStatusCode)
            throw new AssertionFailedException(
                $"Expected statuscode {expectedStatusCode}, but received {response.StatusCode}.\n" +
                $"The response was '{await response.Content.ReadAsStringAsync()}'\n");
    }

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
