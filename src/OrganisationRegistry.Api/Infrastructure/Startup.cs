namespace OrganisationRegistry.Api.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO.Compression;
    using System.Linq;
    using Api.Configuration;
    using Api.Security;
    using App.Metrics.Configuration;
    using App.Metrics.Extensions.Reporting.Graphite;
    using App.Metrics.Extensions.Reporting.Graphite.Client;
    using App.Metrics.Reporting.Interfaces;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.AspNetCore.Mvc.Middleware;
    using Configuration;
    using FluentValidation.AspNetCore;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.Cors.Internal;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Net.Http.Headers;
    using Newtonsoft.Json;
    using Security;
    using Logging;
    using Magda;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Localization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Serilog;
    using SqlServer.Configuration;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Configuration.Database.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using JsonSerializerSettingsProvider = OrganisationRegistry.Infrastructure.Infrastructure.Json.JsonSerializerSettingsProvider;
    using Microsoft.AspNetCore.ResponseCompression;
    using AddHttpSecurityHeadersMiddleware = Security.AddHttpSecurityHeadersMiddleware;

    public class Startup
    {
        private const string AllowSpecificOrigin = "AllowSpecificOrigin";

        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _env;
        private readonly ILoggerFactory _loggerFactory;

        private IContainer _applicationContainer;

        public Startup(
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
                .AddEnvironmentVariables();

            //var keyVaultConfiguration = builder.Build().GetSection(KeyVaultConfiguration.Section).Get<KeyVaultConfiguration>();
            //if (!string.IsNullOrWhiteSpace(keyVaultConfiguration?.Vault))
            //    builder.AddAzureKeyVault(
            //        $"https://{keyVaultConfiguration.Vault}.vault.azure.net/",
            //        keyVaultConfiguration.ClientId,
            //        keyVaultConfiguration.ClientSecret);

            Console.WriteLine("Infrastructure__EventStoreConnectionString = {0}", Environment.GetEnvironmentVariable("Infrastructure__EventStoreConnectionString") ?? "NOT SET");
            Console.WriteLine("Infrastructure__EventStoreAdministrationConnectionString = {0}", Environment.GetEnvironmentVariable("Infrastructure__EventStoreAdministrationConnectionString") ?? "NOT SET");
            Console.WriteLine("Configuration__ConnectionString = {0}", Environment.GetEnvironmentVariable("Configuration__ConnectionString") ?? "NOT SET");
            Console.WriteLine("SqlServer__ConnectionString = {0}", Environment.GetEnvironmentVariable("SqlServer__ConnectionString") ?? "NOT SET");
            Console.WriteLine("SqlServer__MigrationsConnectionString = {0}", Environment.GetEnvironmentVariable("SqlServer__MigrationsConnectionString") ?? "NOT SET");

            var sqlConfiguration = builder.Build().GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();
            _configuration = builder
                .AddEntityFramework(x =>
                    x.UseSqlServer(
                        sqlConfiguration.ConnectionString,
                        y => y.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")))
                .Build();

            _env = env;
            _loggerFactory = loggerFactory;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var apiConfiguration = _configuration.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>();
            var togglesConfiguration = _configuration.GetSection(TogglesConfiguration.Section).Get<TogglesConfiguration>();

            Migrations.Run(_configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>());
            var openIdConfiguration = _configuration.GetSection(OpenIdConnectConfiguration.Section).Get<OpenIdConnectConfiguration>();

            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters =
                        new OrganisationRegistryTokenValidationParameters(openIdConfiguration);
                });

            services
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                //.AddTransient<IPrincipal>(provider => provider.GetService<IHttpContextAccessor>().HttpContext.User)
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddScoped<ISecurityService, SecurityService>()

                .AddMvc(options =>
                {
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                    options.FormatterMappings.SetMediaTypeMappingForFormat("xml", MediaTypeHeaderValue.Parse("application/xml"));
                    options.OutputFormatters.Add(new CsvOutputFormatter(new CsvFormatterOptions()));
                    options.FormatterMappings.SetMediaTypeMappingForFormat("csv", MediaTypeHeaderValue.Parse("text/csv"));
                    //options.Filters.Add(new LoggingFilterFactory());
                    options.Filters.Add(new CorsAuthorizationFilterFactory(AllowSpecificOrigin));
                    options.AddMetricsResourceFilter();
                })

                .AddJsonOptions(options => options.SerializerSettings.ConfigureForOrganisationRegistry())
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .Services

                .AddResponseCompression(options =>
                {
                    options.Providers.Add<GzipCompressionProvider>();

                    options.EnableForHttps = true;

                    options.MimeTypes = new[]
                    {
                        // General
                        "text/plain",

                        // Static files
                        "text/css",
                        "application/javascript",

                        // MVC
                        "text/html",
                        "application/xml",
                        "text/xml",
                        "application/json",
                        "text/json",

                        // Fonts
                        "application/font-woff",
                        "font/otf",
                        "application/vnd.ms-fontobject"
                    };
                })

                .Configure<GzipCompressionProviderOptions>(options =>
                {
                    options.Level = CompressionLevel.Fastest;
                })

                .AddApiVersioning(x => x.ReportApiVersions = true)

                //.AddSwagger(apiConfiguration)

                .AddCors(options => options.AddPolicy(AllowSpecificOrigin, corsPolicy => corsPolicy
                    .WithOrigins(GetCorsOrigins(apiConfiguration, _env))
                    .WithMethods("GET", "POST", "PUT", "HEAD", "DELETE")
                    .WithHeaders("accept", "content-type", "origin", "x-filtering", "x-sorting", "x-pagination",
                        "authorization")
                    .WithExposedHeaders("location", "x-filtering", "x-sorting", "x-pagination")
                    .SetPreflightMaxAge(TimeSpan.FromSeconds(60 * 15))
                    .AllowCredentials()))

                .AddMetrics(options =>
                {
                    options.MetricsEnabled = togglesConfiguration.EnableMonitoring;
                    options.ReportingEnabled = togglesConfiguration.EnableMonitoring;

                    options.WithGlobalTags((globalTags, envInfo) =>
                    {
                        globalTags.Add("server", envInfo.MachineName);
                        globalTags.Add("app", envInfo.EntryAssemblyName);
                        globalTags.Add("env", apiConfiguration.EnvironmentName);
                    });
                })
                .AddJsonSerialization() //Enables json format on the /metrics-text, /metrics, /health and /env endpoints.
                .AddGraphiteMetricsSerialization() // Enables graphite plain text format on the /metrics endpoint.
                .AddGraphiteMetricsTextSerialization() // Enables json format on the /metrics endpoint.
                .AddJsonHealthSerialization() // Enables json format on the /health endpont.
                .AddJsonEnvironmentInfoSerialization() // Enables json format on the /env endpont.
                .AddHealthChecks()
                .AddMetricsMiddleware()
                .AddReporting(factory => factory
                    .AddGraphite(new GraphiteReporterSettings
                    {
                        HttpPolicy = new HttpPolicy
                        {
                            FailuresBeforeBackoff = 3,
                            BackoffPeriod = TimeSpan.FromSeconds(30),
                            Timeout = TimeSpan.FromSeconds(3)
                        },
                        GraphiteSettings = new GraphiteSettings(new Uri(apiConfiguration.GraphiteAddress)),
                        ReportInterval = TimeSpan.FromSeconds(5)
                    }));

            services.AddOptions();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new MagdaModule(_configuration));
            builder.RegisterModule(new ApiModule(_configuration, services));
            _applicationContainer = builder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        private static string[] GetCorsOrigins(ApiConfiguration apiConfiguration, IHostingEnvironment env)
        {
            var corsOrigins = apiConfiguration.CorsOrigin.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (env.IsDevelopment())
            {
                corsOrigins = corsOrigins
                    .Append("http://localhost:3000")
                    .Append("https://wegwijs.dev.informatievlaanderen.be:3000")
                    .ToArray();
            }
            return corsOrigins;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            ILoggerFactory loggerFactory,
            IApplicationLifetime appLifetime/*,
            OrganisationRegistryTokenValidationParameters tokenValidationParameters*/)
        {
            var defaultCulture = new CultureInfo("nl-BE") { DateTimeFormat = { FirstDayOfWeek = DayOfWeek.Monday } };

            app
                .ConfigureLogging()
                .InitialiseAndUpdateDatabase()

                .UseCors(policyName: AllowSpecificOrigin)

                .UseMiddleware<ApplicationStatusMiddleware>()
                .UseMiddleware<EnableRequestRewindMiddleware>()
                .UseMiddleware<AddCorrelationIdToLogContextMiddleware>()
                .UseMiddleware<AddCorrelationIdToResponseMiddleware>()
                .UseMiddleware<AddHttpSecurityHeadersMiddleware>()
                .UseMiddleware<AddVersionHeaderMiddleware>("x-wegwijs-version")
                .UseMiddleware<ConfigureClaimsPrincipalSelectorMiddleware>()

                .UseOrganisationRegistryExceptionHandler(loggerFactory)
                .UseOrganisationRegistryEventSourcing()

                //.UseOrganisationRegistryCookieAuthentication(tokenValidationParameters)
                //.UseOrganisationRegistryJwtBearerAuthentication(tokenValidationParameters)
                .UseAuthentication()

                .UseMetrics()
                .UseMetricsReporting(appLifetime)

                .UseRequestLocalization(new RequestLocalizationOptions
                {
                    DefaultRequestCulture = new RequestCulture(defaultCulture),
                    SupportedCultures = new List<CultureInfo>
                    {
                        defaultCulture
                    },
                    SupportedUICultures = new List<CultureInfo>
                    {
                        defaultCulture
                    }
                })

                .UseMvc()

                .UseMiddleware<DefaultResponseCompressionQualityMiddleware>(new Dictionary<string, double>
                {
                    {"br", 1.0},
                    {"gzip", 0.9}
                })
                .UseResponseCompression();

                // swagger/v1/swagger.json -> docs/v1/docs.json
                //.UseSwagger("docs/{documentName}/docs.json");

            appLifetime.ApplicationStopped.Register(() =>
            {
                Log.CloseAndFlush();
                _applicationContainer.Dispose();
            });
        }
    }
}
