namespace OrganisationRegistry.Api.Infrastructure;

using System.IO;
using System.Security.Cryptography.X509Certificates;
using Be.Vlaanderen.Basisregisters.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using OrganisationRegistry.Configuration.Database;
using OrganisationRegistry.Configuration.Database.Configuration;
using OrganisationRegistry.Infrastructure;

public class Program
{
    public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .UseDefaultForApi<Startup>(
                new ProgramOptions
                {
                    Hosting =
                    {
                        HttpPort = 9002,
                        HttpsPort = 9003,
                        HttpsCertificate =
                            () => new X509Certificate2(Path.Join("..", "..", "organisationregistry-api.pfx"), "organisationregistry"),
                    },
                    Logging =
                    {
                        WriteTextToConsole = false,
                        WriteJsonToConsole = false,
                    },
                    Runtime =
                    {
                        CommandLineArgs = args,
                    },
                    MiddlewareHooks =
                    {
                        ConfigureAppConfiguration = (_, config) =>
                        {
                            var sqlConfiguration = config
                                .Build()
                                .GetSection(ConfigurationDatabaseConfiguration.Section)
                                .Get<ConfigurationDatabaseConfiguration>();

                            config
                                .AddEntityFramework(x =>
                                    x.UseSqlServer(
                                        sqlConfiguration.ConnectionString,
                                        y => y.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema)))
                                .Build();
                        },
                    },
                });
}
