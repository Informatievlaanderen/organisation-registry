namespace OrganisationRegistry.Api.IntegrationTests
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Security.Cryptography.X509Certificates;
    using Infrastructure;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using SqlServer.Configuration;
    using SqlServer.Infrastructure;

    public class ApiFixture : IDisposable
    {
        private readonly IWebHost _webHost;
        public const string ApiEndpoint = "http://localhost:5000/v1/";
        public HttpClient HttpClient { get; } = new HttpClient { BaseAddress = new Uri(ApiEndpoint) };

        public ApiFixture()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(typeof(Startup).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true);

            var connectionString =
                builder.Build()
                .GetSection(SqlServerConfiguration.Section)
                .Get<SqlServerConfiguration>()
                .MigrationsConnectionString;

            var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseSqlServer(
                    connectionString,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"))
                .Options;

            new OrganisationRegistryContext(dbContextOptions).Database.EnsureDeleted();

            IWebHostBuilder hostBuilder = new WebHostBuilder();
            var environment = hostBuilder.GetSetting("environment");

            if (environment == "Development")
            {
                var cert = new X509Certificate2("organisationregistry-api.pfx", "organisationregistry");

                hostBuilder = hostBuilder
                    .UseKestrel(server =>
                    {
                        server.AddServerHeader = false;
                        server.Listen(IPAddress.Any, 2443, listenOptions => listenOptions.UseHttps(cert));
                    })
                    .UseUrls("https://api.organisatie.dev-basisregisters.vlaanderen:2443");
            }
            else
            {
                hostBuilder = hostBuilder.UseKestrel(server => server.AddServerHeader = false);
            }

            _webHost = hostBuilder
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            _webHost.Start();

            //Import.Piavo.Program.Import(
            //    ApiEndpoint,
            //    "dwegwijs_api",
            //    "");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _webHost?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
