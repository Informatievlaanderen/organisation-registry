namespace OrganisationRegistry.Api.Infrastructure;

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Api.Security;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Be.Vlaanderen.Basisregisters.Api;
using Be.Vlaanderen.Basisregisters.Api.Search.Filtering;
using Be.Vlaanderen.Basisregisters.Api.Search.Pagination;
using Be.Vlaanderen.Basisregisters.Api.Search.Sorting;
using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
using Configuration;
using global::OpenTelemetry.Trace;
using HostedServices;
using Magda;
using Microsoft.AspNetCore.Authentication;
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
using OpenTelemetry.Extensions;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Cache;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Infrastructure.Json;
using OrganisationRegistry.Magda;
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

    private IContainer? _applicationContainer;
    private readonly ILogger<Startup> _logger;

    public Startup(
        IConfiguration configuration,
        ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _logger = _loggerFactory.CreateLogger<Startup>();
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
        var bffApiEnabled = _configuration.GetValue<bool>($"FeatureManagement:{FeatureFlags.BffApi}");

        if (apiConfiguration.KboCertificate is { } kboCertificate && kboCertificate.IsNotEmptyOrWhiteSpace())
        {
            MagdaClientCertificate? clientCertificate = null;
            try
            {
                clientCertificate = MagdaClientCertificate.Create(
                    kboCertificate,
                    apiConfiguration.RijksRegisterCertificatePwd);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex,
                    "##########################################################################################\n" +
                    "# MAGDA CERTIFICATE INVALID: KboCertificate is set but could not be loaded.             #\n" +
                    "# The application will start but all Magda calls will fail.                             #\n" +
                    "# Verify that ApiConfiguration:KboCertificate contains a valid base-64 encoded PFX.    #\n" +
                    "##########################################################################################");
            }

            if (clientCertificate is not null)
            {
                services
                    .AddHttpClient()
                    .AddHttpClient(MagdaModule.HttpClientName)
                    .ConfigurePrimaryHttpMessageHandler(() => new MagdaHttpClientHandler(clientCertificate));
            }
            else
            {
                services
                    .AddHttpClient()
                    .AddHttpClient(MagdaModule.HttpClientName)
                    .ConfigurePrimaryHttpMessageHandler(() => new InvalidCertificateMagdaHttpClientHandler());
            }
        }
        else
        {
            services
                .AddHttpClient()
                .AddHttpClient(MagdaModule.HttpClientName);

            _logger.LogWarning("Magda clientcertificate not configured");
        }

        var authBuilder = services
            .AddHostedService<ScheduledCommandsService>()
            .AddHostedService<SyncFromKboService>()
            .AddHostedService<SyncRemovedItemsService>()
            .AddHostedService<ProcessImportedFilesService>()
            .AddHostedService<MEPCalculatorService>()
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
                });

        if (bffApiEnabled)
            authBuilder.AddOAuth2Introspection(
                AuthenticationSchemes.BffApi,
                options =>
                {
                    options.ClientId = editApiConfiguration.ClientId;
                    options.ClientSecret = editApiConfiguration.ClientSecret;
                    options.Authority = editApiConfiguration.Authority;
                    options.IntrospectionEndpoint = editApiConfiguration.IntrospectionEndpoint;
                });

        authBuilder
            .Services
            .AddTransient<IClaimsTransformation, Security.BffClaimsTransformation>()
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
                        ExposedHeaders = new[] { SearchConstants.SearchMetaDataHeaderName },
                        Headers =
                            new []
                            {
                                HeaderNames.Accept,
                                HeaderNames.ContentType,
                                HeaderNames.Origin,
                                HeaderNames.Authorization,
                                HeaderNames.IfMatch,
                                ExtractFilteringRequestExtension.HeaderName,
                                AddSortingExtension.HeaderName,
                                AddPaginationExtension.HeaderName,
                                "traceparent",
                                "tracestate"
                            },
                    },
                    Localization =
                    {
                        DefaultCulture = new CultureInfo("nl-BE")
                            { DateTimeFormat = { FirstDayOfWeek = DayOfWeek.Monday } },
                    },
                    Swagger =
                    {
                        MiddlewareHooks =
                        {
                            AfterSwaggerGen = x =>
                            {
                                x.EnableAnnotations();
                                x.OperationFilter<ProblemJsonResponseFilter>();
                                x.OperationFilter<PaginationHeaderOperationFilter>();
                                x.CustomSchemaIds(type => type.ToString());
                                x.DocumentFilter<TagDescriptionDocumentFilter>();
                            },
                        },
                        ApiInfo = (_, description) => new OpenApiInfo
                        {
                            Version = description.ApiVersion.ToString(),
                            Title = "Basisregisters Vlaanderen Organisation Registry API",
                            Description = GetApiLeadingText(description),
                            Contact = new OpenApiContact
                            {
                                Name = "Digitaal Vlaanderen",
                                Email = "digitaal.vlaanderen@vlaanderen.be",
                                Url = new Uri("https://legacy.basisregisters.vlaanderen"),
                            },
                        },
                        XmlCommentPaths = new[]
                        {
                            typeof(Startup).GetTypeInfo().Assembly.GetName().Name,
                        }!,
                    },
                    Server =
                    {
                        VersionHeaderName = "x-wegwijs-version",
                    },
                    MiddlewareHooks =
                    {
                        Authorization = options =>
                        {
                            options.AddPolicy(
                                PolicyNames.Organisations,
                                builder => builder.RequireClaim(
                                    AcmIdmConstants.Claims.Scope,
                                    AcmIdmConstants.Scopes.CjmBeheerder,
                                    AcmIdmConstants.Scopes.TestClient));

                            options.AddPolicy(
                                PolicyNames.BankAccounts,
                                builder => builder.RequireClaim(
                                    AcmIdmConstants.Claims.Scope,
                                    AcmIdmConstants.Scopes.CjmBeheerder,
                                    AcmIdmConstants.Scopes.TestClient));

                            options.AddPolicy(
                                PolicyNames.OrganisationClassifications,
                                builder => builder.RequireClaim(
                                    AcmIdmConstants.Claims.Scope,
                                    AcmIdmConstants.Scopes.CjmBeheerder,
                                    AcmIdmConstants.Scopes.TestClient));

                            options.AddPolicy(
                                PolicyNames.OrganisationContacts,
                                builder => builder.RequireClaim(
                                    AcmIdmConstants.Claims.Scope,
                                    AcmIdmConstants.Scopes.CjmBeheerder,
                                    AcmIdmConstants.Scopes.TestClient));

                            options.AddPolicy(
                                PolicyNames.Keys,
                                builder => builder.RequireClaim(
                                    AcmIdmConstants.Claims.Scope,
                                    AcmIdmConstants.Scopes.CjmBeheerder,
                                    AcmIdmConstants.Scopes.OrafinBeheerder,
                                    AcmIdmConstants.Scopes.TestClient)
                            );
                        },
                        ConfigureJsonOptions = options => { options.SerializerSettings.ConfigureForOrganisationRegistry(); },
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
                        },
                    },
                });

        services.AddOpenTelemetry(
            builder => builder
                .AddHttpClientInstrumentation()
                .AddAspNetCoreWithDistributedTracing());

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
                        LoggerFactory = loggerFactory,
                    },
                    Toggles =
                    {
                        Enable = datadogToggle,
                        Debug = debugDataDogToggle,
                    },
                    Tracing =
                    {
                        TraceIdHeaderName = "traceid",
                        ParentSpanIdHeaderName = "traceparent",
                        ServiceName = _configuration["DataDog:ServiceName"],
                        LogForwardedForEnabled = true,
                    },
                })
            .UseDefaultForApi(
                new StartupUseOptions
                {
                    Common =
                    {
                        ApplicationContainer = _applicationContainer!, // if _applicationContainer is null here, something is off
                        ServiceProvider = serviceProvider,
                        HostingEnvironment = env,
                        ApplicationLifetime = appLifetime,
                        LoggerFactory = loggerFactory,
                    },
                    Api =
                    {
                        VersionProvider = apiVersionProvider,
                        Info = groupName => $"Basisregisters Vlaanderen - Organisation Registry API {groupName}",
                        CSharpClientOptions =
                        {
                            ClassName = "OrganisationRegistry",
                            Namespace = "Be.Vlaanderen.Basisregisters",
                        },
                        TypeScriptClientOptions =
                        {
                            ClassName = "OrganisationRegistry",
                        },
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
                            .UseAuthentication(),
                    },
                });
    }

    private const string SchermApiIntro =
        "\n\n" +
        "## Scherm APIs \u2014 algemene werking\n\n" +
        "De **Scherm APIs** zijn de interne beheers-API's van het Organisatieregister van Digitaal Vlaanderen. " +
        "Ze zijn uitsluitend bedoeld voor gebruik door de beheerinterface en vereisen authenticatie via **ACM/IDM**.\n\n" +
        "---\n\n" +
        "### Authenticatie\n\n" +
        "Alle Scherm API-endpoints vereisen een geldig **Bearer-token** van ACM/IDM. " +
        "Voeg het token toe als HTTP-header:\n\n" +
        "```\nAuthorization: Bearer <token>\n```\n\n" +
        "---\n\n" +
        "### Paginering\n\n" +
        "Lijstendpoints ondersteunen paginering via de `x-pagination` request-header.\n\n" +
        "**Request-header:**\n\n" +
        "| Waarde | Betekenis |\n" +
        "|---|---|\n" +
        "| `1,10` | Pagina 1, 10 items per pagina |\n" +
        "| `2,25` | Pagina 2, 25 items per pagina |\n" +
        "| `none` | Geen paginering \u2014 geeft alle resultaten terug |\n\n" +
        "**Response-header:**\n\n" +
        "De server geeft `x-pagination` terug als JSON-object:\n\n" +
        "```json\n{ \"currentPage\": 1, \"itemsPerPage\": 10, \"totalItems\": 42, \"totalPages\": 5 }\n```\n\n" +
        "---\n\n" +
        "### Sortering\n\n" +
        "Lijstendpoints ondersteunen sortering via de `x-sorting` request-header.\n\n" +
        "**Formaat:** `<richting>,<veldnaam>`\n\n" +
        "| Voorbeeld | Betekenis |\n" +
        "|---|---|\n" +
        "| `Ascending,name` | Oplopend sorteren op veld `name` |\n" +
        "| `Descending,name` | Aflopend sorteren op veld `name` |\n\n" +
        "De beschikbare veldnamen zijn afhankelijk van het endpoint.\n\n" +
        "---\n\n" +
        "### Foutformaat\n\n" +
        "Fouten worden teruggegeven als **RFC 7807 Problem Details**:\n\n" +
        "| Statuscode | Betekenis |\n" +
        "|---|---|\n" +
        "| `200 OK` | Verzoek geslaagd |\n" +
        "| `400 Bad Request` | Ongeldige invoer of validatiefout |\n" +
        "| `404 Not Found` | Resource bestaat niet |\n" +
        "| `500 Internal Server Error` | Onverwachte fout aan serverzijde |\n";

    private static string GetApiLeadingText(ApiVersionDescription description)
        => $"Momenteel leest u de documentatie voor versie {description.ApiVersion} van de Basisregisters Vlaanderen Organisation Registry API{(description.IsDeprecated ? ", **deze API versie is niet meer ondersteund**." : ".")}"
           + SchermApiIntro;
}
