namespace OrganisationRegistry.MagdaReRegistration
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Api.Infrastructure.Search;
    using Api.Kbo;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Autofac.Features.OwnedInstances;
    using Configuration;
    using Destructurama;
    using global::Magda.RegistreerInschrijving;
    using Infrastructure.Authorization;
    using Infrastructure.Bus;
    using Infrastructure.Configuration;
    using Infrastructure.Infrastructure.Json;
    using Magda;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;
    using Serilog;
    using SqlServer.Infrastructure;
    using ILogger = Microsoft.Extensions.Logging.ILogger;

    internal class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Starting Magda Reregistrations");

            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .AddEnvironmentVariables();

            var configuration = builder.Build();

            var app = ConfigureServices(configuration);

            var logger = app.GetService<ILogger<Program>>();

            await RunProgram(logger, app);
        }

        private static async Task RunProgram(
            ILogger logger,
            IServiceProvider app)
        {
            var options = app.GetService<IOptions<MagdaReRegistrationConfiguration>>().Value;
            var magdaOptions = app.GetService<MagdaConfiguration>();

            var allOrganisations = await FetchOrganisations(logger, options);

            var registerInscriptionCommand = new RegistreerInschrijvingCommand(
                app.GetService<Func<Owned<OrganisationRegistryContext>>>(),
                magdaOptions,
                app.GetService<IHttpClientFactory>(),
                app.GetService<ILogger<RegistreerInschrijvingCommand>>());

            var claimsIdentity = new User("Magda", "Reregistrator", "Magda Reregistrator", null, new[]
            {
                Role.AutomatedTask
            });

            foreach (var organisation in allOrganisations)
            {
                await RegisterOrganisation(
                    logger,
                    registerInscriptionCommand,
                    organisation,
                    claimsIdentity);
            }
        }

        private static async Task RegisterOrganisation(
            ILogger logger,
            IRegistreerInschrijvingCommand registerInscriptionCommand,
            OrganisationResult organisation, IUser user)
        {
            var envelope = await registerInscriptionCommand.Execute(
                user,
                organisation.KboNumber);

            if (envelope == null)
                throw new Exception("Geen antwoord van magda gekregen.");

            var reply = envelope.Body?.RegistreerInschrijvingResponse?.Repliek?.Antwoorden?.Antwoord;
            if (reply == null)
                throw new Exception("Geen antwoord van magda gekregen.");

            reply.Uitzonderingen?
                .Where(type => type.Type == UitzonderingTypeType.INFORMATIE)
                .ToList()
                .ForEach(type => logger.LogInformation($"{type.Diagnose}"));

            reply.Uitzonderingen?
                .Where(type => type.Type == UitzonderingTypeType.WAARSCHUWING)
                .ToList()
                .ForEach(type => logger.LogWarning($"{type.Diagnose}"));

            var errors = reply.Uitzonderingen?
                .Where(type => type.Type == UitzonderingTypeType.FOUT)
                ?.ToList() ?? new List<UitzonderingType>();

            errors.ForEach(type => logger.LogError($"{type.Diagnose}"));

            if (errors.Any())
                throw new Exception(
                    $"Fout in magda response:\n" +
                    $"{string.Join('\n', errors.Select(type => type.Diagnose))}");
        }

        private static async Task<List<OrganisationResult>> FetchOrganisations(
            ILogger logger,
            MagdaReRegistrationConfiguration options)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(options.OrganisationRegistryApiUri)
            };

            var response =
                await client.GetAsync("/v1/search/organisations?q=(_exists_:kboNumber)&fields=name,kboNumber&scroll=true");
            response.EnsureSuccessStatusCode();

            var searchResponseHeader =
                JsonConvert.DeserializeObject<SearchResponseHeader>(response.Headers.GetValues(SearchConstants.SearchMetaDataHeaderName).First());

            var content = await response.Content.ReadAsStringAsync();
            var organisations = JsonConvert.DeserializeObject<List<OrganisationResult>>(content);

            var allOrganisations = new List<OrganisationResult>();

            allOrganisations.AddRange(organisations);

            while (organisations.Any())
            {
                var scrollResponse =
                    await client.GetAsync($"/v1/search/organisations/scroll?id={searchResponseHeader.ScrollId}");

                scrollResponse.EnsureSuccessStatusCode();

                searchResponseHeader =
                    JsonConvert.DeserializeObject<SearchResponseHeader>(response.Headers.GetValues(SearchConstants.SearchMetaDataHeaderName).First());

                content = await scrollResponse.Content.ReadAsStringAsync();
                organisations = JsonConvert.DeserializeObject<List<OrganisationResult>>(content);

                allOrganisations.AddRange(organisations);

                logger.LogInformation(
                    $"Total organisations: {allOrganisations.Count} (Last kbo: {allOrganisations.Last().KboNumber})");
            }

            return allOrganisations;
        }

        private static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            services.AddLogging(loggingBuilder =>
            {
                var loggerConfiguration = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .Enrich.FromLogContext()
                    .Enrich.WithMachineName()
                    .Enrich.WithThreadId()
                    .Enrich.WithEnvironmentUserName()
                    .Destructure.JsonNetTypes();

                Serilog.Debugging.SelfLog.Enable(Console.WriteLine);

                Log.Logger = loggerConfiguration.CreateLogger();

                loggingBuilder.AddSerilog();
            });

            var apiConfiguration = configuration.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>();

            var magdaClientCertificate = MagdaClientCertificate.Create(
                apiConfiguration.KboCertificate,
                apiConfiguration.RijksRegisterCertificatePwd);

            services
                .AddOptions()
                .AddHttpClient()
                .AddHttpClient(MagdaModule.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new MagdaHttpClientHandler(magdaClientCertificate));

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new MagdaReRegistrationRunnerModule(configuration, services, serviceProvider.GetService<ILoggerFactory>()));
            return new AutofacServiceProvider(builder.Build());
        }
    }

    internal class SearchResponseHeader
    {
        public string ScrollId { get; set; }
    }

    internal class OrganisationResult
    {
        public string KboNumber { get; set; }
    }
}
