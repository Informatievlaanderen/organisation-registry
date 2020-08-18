namespace OrganisationRegistry.MagdaReRegistration
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using ElasticSearch;
    using Infrastructure;
    using Infrastructure.Events;
    using Magda;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using SqlServer;

    public class MagdaReRegistrationRunnerModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public MagdaReRegistrationRunnerModule(
            IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder
                .RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services))
                .RegisterModule(new OrganisationRegistryModule())
                .RegisterModule(new ElasticSearchModule(_configuration, _services))
                .RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory))
                .RegisterModule(new MagdaModule(_configuration))
                .RegisterModule(new MagdaReRegistrationModule(_configuration, _services));

            builder.Populate(_services);
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();
            return () => defaultServiceProvider.CreateScope().ServiceProvider;
        }
    }
}
