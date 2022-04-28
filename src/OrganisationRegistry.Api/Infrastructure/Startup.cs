namespace OrganisationRegistry.Api.Infrastructure
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using Api.Security;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Backoffice.Kbo;
    using Be.Vlaanderen.Basisregisters.Api;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Configuration;
    using HostedServices;
    using Magda;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc.ApiExplorer;
    using Microsoft.AspNetCore.Mvc.Infrastructure;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Diagnostics.HealthChecks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Microsoft.FeatureManagement;
    using Microsoft.Net.Http.Headers;
    using Microsoft.OpenApi.Models;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Authorization.Cache;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using Search;
    using Security;
    using SqlServer.Configuration;
    using SqlServer.Infrastructure;
    using Swagger;

    public class Startup
    {
        private const string DatabaseTag = "db";

        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        private IContainer _applicationContainer;

        public Startup(
            IConfiguration configuration,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>Configures services for the application.</summary>
        /// <param name="services">The collection of services to configure the application with.</param>
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            Migrations.Run(_configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>());
            var openIdConfiguration = _configuration.GetSection(OpenIdConnectConfigurationSection.Name)
                .Get<OpenIdConnectConfigurationSection>();
            var apiConfiguration =
                _configuration.GetSection(ApiConfigurationSection.Name).Get<ApiConfigurationSection>();
            var editApiConfiguration = _configuration.GetSection(EditApiConfigurationSection.Name)
                .Get<EditApiConfigurationSection>();

            var magdaClientCertificate = MagdaClientCertificate.Create(
                apiConfiguration.KboCertificate,
                apiConfiguration.RijksRegisterCertificatePwd);

            services
                .AddHostedService<ScheduledCommandsService>()
                .AddHostedService<SyncFromKboService>()
                .AddAuthentication(
                    options =>
                    {
                        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    })
                .AddJwtBearer(
                    JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters =
                            new OrganisationRegistryTokenValidationParameters(openIdConfiguration);
                    })
                .AddOAuth2Introspection(
                    AuthenticationSchemes.EditApi,
                    options =>
                    {
                        options.ClientId = editApiConfiguration.ClientId;
                        options.ClientSecret = editApiConfiguration.ClientSecret;
                        options.Authority = editApiConfiguration.Authority;
                        options.IntrospectionEndpoint = editApiConfiguration.IntrospectionEndpoint;
                    })
                .Services
                .AddSingleton<IActionContextAccessor, ActionContextAccessor>()
                .AddSingleton<ISecurityService, SecurityService>()
                .AddSingleton<ICache<OrganisationSecurityInformation>, OrganisationSecurityCache>()
                .AddSingleton<IOrganisationRegistryConfiguration>(
                    new OrganisationRegistryConfiguration(
                        _configuration
                            .GetSection(ApiConfigurationSection.Name)
                            .Get<ApiConfigurationSection>(),
                        _configuration
                            .GetSection(OrganisationTerminationConfigurationSection.Name)
                            .Get<OrganisationTerminationConfigurationSection>(),
                        _configuration
                            .GetSection(AuthorizationConfigurationSection.Name)
                            .Get<AuthorizationConfigurationSection>(),
                        _configuration
                            .GetSection(CachingConfigurationSection.Name)
                            .Get<CachingConfigurationSection>(),
                        _configuration
                            .GetSection(HostedServicesConfigurationSection.Name)
                            .Get<HostedServicesConfigurationSection>()))
                .AddFeatureManagement()
                .Services
                .AddHttpClient()
                .AddHttpClient(MagdaModule.HttpClientName)
                .ConfigurePrimaryHttpMessageHandler(() => new MagdaHttpClientHandler(magdaClientCertificate))
                .Services
                .ConfigureDefaultForApi<Startup>(
                    new StartupConfigureOptions
                    {
                        Cors =
                        {
                            Origins = _configuration
                                .GetSection("Cors")
                                .GetChildren()
                                .Select(c => c.Value)
                                .ToArray(),
                            ExposedHeaders = new[] { SearchConstants.SearchMetaDataHeaderName }
                        },
                        Localization =
                        {
                            DefaultCulture = new CultureInfo("nl-BE")
                                { DateTimeFormat = { FirstDayOfWeek = DayOfWeek.Monday } }
                        },
                        Swagger =
                        {
                            MiddlewareHooks =
                            {
                                AfterSwaggerGen = x =>
                                {
                                    x.EnableAnnotations();
                                    x.OperationFilter<ProblemJsonResponseFilter>();
                                }
                            },
                            ApiInfo = (provider, description) => new OpenApiInfo
                            {
                                Version = description.ApiVersion.ToString(),
                                Title = "Basisregisters Vlaanderen Organisation Registry API",
                                Description = GetApiLeadingText(description),
                                Contact = new OpenApiContact
                                {
                                    Name = "Digitaal Vlaanderen",
                                    Email = "digitaal.vlaanderen@vlaanderen.be",
                                    Url = new Uri("https://legacy.basisregisters.vlaanderen")
                                }
                            },
                            XmlCommentPaths = new[]
                            {
                                typeof(Startup).GetTypeInfo().Assembly.GetName().Name
                            }!
                        },
                        Server =
                        {
                            VersionHeaderName = "x-wegwijs-version"
                        },
                        MiddlewareHooks =
                        {
                            ConfigureJsonOptions = options =>
                            {
                                options.SerializerSettings.ConfigureForOrganisationRegistry();
                            },
                            ConfigureMvcCore = cfg =>
                            {
                                cfg.OutputFormatters.Add(new CsvOutputFormatter(new CsvFormatterOptions()));
                                cfg.FormatterMappings.SetMediaTypeMappingForFormat(
                                    "csv",
                                    MediaTypeHeaderValue.Parse("text/csv"));
                                cfg.EnableEndpointRouting = false;
                            },
                            AfterMvc = builder => builder
                                .AddFormatterMappings()
                                .ConfigureApiBehaviorOptions(
                                    options => { options.SuppressModelStateInvalidFilter = true; }),
                            AfterHealthChecks = health =>
                            {
                                var connectionStrings = _configuration
                                    .GetSection("ConnectionStrings")
                                    .GetChildren();

                                foreach (var connectionString in connectionStrings)
                                    health.AddSqlServer(
                                        connectionString.Value,
                                        name: $"sqlserver-{connectionString.Key.ToLowerInvariant()}",
                                        tags: new[] { DatabaseTag, "sql", "sqlserver" });

                                health.AddDbContextCheck<OrganisationRegistryContext>(
                                    $"dbcontext-{nameof(OrganisationRegistryContext).ToLowerInvariant()}",
                                    tags: new[] { DatabaseTag, "sql", "sqlserver" });
                            }
                        }
                    });

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new MagdaModule(_configuration));
            containerBuilder.RegisterModule(new ApiModule(_configuration, services, _loggerFactory));
            _applicationContainer = containerBuilder.Build();

            return new AutofacServiceProvider(_applicationContainer);
        }

        public void Configure(
            IServiceProvider serviceProvider,
            IApplicationBuilder app,
            IWebHostEnvironment env,
            IHostApplicationLifetime appLifetime,
            ILoggerFactory loggerFactory,
            IApiVersionDescriptionProvider apiVersionProvider,
            ApiDataDogToggle datadogToggle,
            ApiDebugDataDogToggle debugDataDogToggle,
            HealthCheckService healthCheckService)
        {
            StartupHelpers.CheckDatabases(healthCheckService, DatabaseTag, loggerFactory).GetAwaiter().GetResult();

            app
                .UseDataDog<Startup>(
                    new DataDogOptions
                    {
                        Common =
                        {
                            ServiceProvider = serviceProvider,
                            LoggerFactory = loggerFactory
                        },
                        Toggles =
                        {
                            Enable = datadogToggle,
                            Debug = debugDataDogToggle
                        },
                        Tracing =
                        {
                            ServiceName = _configuration["DataDog:ServiceName"],
                            LogForwardedForEnabled = true
                        }
                    })
                .UseDefaultForApi(
                    new StartupUseOptions
                    {
                        Common =
                        {
                            ApplicationContainer = _applicationContainer,
                            ServiceProvider = serviceProvider,
                            HostingEnvironment = env,
                            ApplicationLifetime = appLifetime,
                            LoggerFactory = loggerFactory
                        },
                        Api =
                        {
                            VersionProvider = apiVersionProvider,
                            Info = groupName => $"Basisregisters Vlaanderen - Organisation Registry API {groupName}",
                            CSharpClientOptions =
                            {
                                ClassName = "OrganisationRegistry",
                                Namespace = "Be.Vlaanderen.Basisregisters"
                            },
                            TypeScriptClientOptions =
                            {
                                ClassName = "OrganisationRegistry"
                            }
                        },
                        MiddlewareHooks =
                        {
                            AfterMiddleware = x => x
                                .UseMiddleware<ApplicationStatusMiddleware>()
                                .UseMiddleware<AddNoCacheHeadersMiddleware>()
                                .UseMiddleware<ConfigureClaimsPrincipalSelectorMiddleware>(),

                            AfterHealthChecks = x => x
                                .InitialiseAndUpdateDatabase()
                                .UseOrganisationRegistryExceptionHandler(loggerFactory)
                                .UseOrganisationRegistryEventSourcing()
                                //.UseOrganisationRegistryCookieAuthentication(tokenValidationParameters)
                                //.UseOrganisationRegistryJwtBearerAuthentication(tokenValidationParameters)
                                .UseAuthentication()
                        }
                    });
        }

        private static string GetApiLeadingText(ApiVersionDescription description)
            => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Organisation Registry API{string.Format(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund * *." : ".")}";
    }
}
