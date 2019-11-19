namespace OrganisationRegistry.Api
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Configuration;
    using ElasticSearch;
    using Infrastructure.Search;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SqlServer;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Organisation;

    public class ApiModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;

            _services
                .Configure<ApiConfiguration>(_configuration.GetSection(ApiConfiguration.Section))
                .Configure<OpenIdConnectConfiguration>(_configuration.GetSection(OpenIdConnectConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services))
                .RegisterModule(new OrganisationRegistryModule())
                .RegisterModule(new ElasticSearchModule(_configuration, _services))
                .RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory))
                .RegisterModule(new ConfigurationDatabaseModule(_configuration, _services, _loggerFactory))
                .RegisterModule(new DataDogModule(_configuration));

            builder
                .RegisterType<SearchConstants>().SingleInstance();

            builder
                .RegisterType<OrganisationRegistryTokenValidationParameters>().SingleInstance();

            builder
                .RegisterAssemblyTypes(typeof(OrganisationRegistryApiAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder
                .Register(context =>
                    new OrganisationRegistryConfiguration(_configuration.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>()))
                .As<IOrganisationRegistryConfiguration>()
                .SingleInstance();

            builder
                .Populate(_services);
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();

            if (!context.TryResolve(out IHttpContextAccessor httpContextAccessor))
                return () => defaultServiceProvider.CreateScope().ServiceProvider;

            return () =>
            {
                var requestServices = httpContextAccessor?.HttpContext?.RequestServices;
                return requestServices ?? defaultServiceProvider.CreateScope().ServiceProvider;
            };
        }
    }
}
