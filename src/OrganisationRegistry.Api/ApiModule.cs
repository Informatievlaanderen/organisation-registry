namespace OrganisationRegistry.Api
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using ElasticSearch;
    using Infrastructure.Search;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SqlServer;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Organisation;

    public class ApiModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public ApiModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;

            _services.Configure<ApiConfiguration>(_configuration.GetSection(ApiConfiguration.Section));
            _services.Configure<OpenIdConnectConfiguration>(_configuration.GetSection(OpenIdConnectConfiguration.Section));
            _services.Configure<AuthConfiguration>(_configuration.GetSection(AuthConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
            builder.RegisterModule(new OrganisationRegistryModule());
            builder.RegisterModule(new ElasticSearchModule(_configuration, _services));
            builder.RegisterModule(new SqlServerModule(_configuration, _services));
            builder.RegisterModule(new ConfigurationDatabaseModule(_configuration, _services));

            builder.RegisterType<SearchConstants>().SingleInstance();
            builder.RegisterType<OrganisationRegistryTokenValidationParameters>().SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryApiAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsImplementedInterfaces();

            builder.Register(context =>
                    new OrganisationRegistryConfiguration(_configuration.GetSection(ApiConfiguration.Section).Get<ApiConfiguration>()))
                .As<IOrganisationRegistryConfiguration>()
                .SingleInstance();

            builder.Populate(_services);
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
