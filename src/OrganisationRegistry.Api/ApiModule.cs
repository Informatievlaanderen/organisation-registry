namespace OrganisationRegistry.Api
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Be.Vlaanderen.Basisregisters.Api.Exceptions;
    using Be.Vlaanderen.Basisregisters.DataDog.Tracing.Autofac;
    using Configuration;
    using ElasticSearch;
    using Infrastructure.Search;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Configuration;
    using SqlServer;
    using OrganisationRegistry.Configuration.Database;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Configuration;
    using ScheduledCommands;

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
                .Configure<ApiConfigurationSection>(_configuration.GetSection(ApiConfigurationSection.Name))
                .Configure<EditApiConfigurationSection>(_configuration.GetSection(EditApiConfigurationSection.Name))
                .Configure<OpenIdConnectConfigurationSection>(_configuration.GetSection(OpenIdConnectConfigurationSection.Name))
                .Configure<AuthorizationConfigurationSection>(_configuration.GetSection(AuthorizationConfigurationSection.Name));
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
                .Except<ScheduledCommandsService>()
                .AsImplementedInterfaces();

            builder.RegisterType<ProblemDetailsHelper>()
                .AsSelf();

            builder
                .Register(_ =>
                    new OrganisationRegistryConfiguration(
                        _configuration
                            .GetSection(ApiConfigurationSection.Name)
                            .Get<ApiConfigurationSection>(),
                        _configuration
                            .GetSection(OrganisationTerminationConfigurationSection.Name)
                            .Get<OrganisationTerminationConfigurationSection>(),
                        _configuration
                            .GetSection(AuthorizationConfigurationSection.Name)
                            .Get<AuthorizationConfigurationSection>()))
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
