namespace OrganisationRegistry.AgentschapZorgEnGezondheid.FtpDump
{
    using System;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using ElasticSearch;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using SqlServer;
    using Infrastructure;
    using Infrastructure.Events;
    using Microsoft.Extensions.Logging;

    public class AgentschapZorgEnGezondheidFtpDumpModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public AgentschapZorgEnGezondheidFtpDumpModule(
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
                .RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory));

            builder.RegisterInstance<IConfigureOptions<AgentschapZorgEnGezondheidFtpDumpConfiguration>>(
                    new ConfigureFromConfigurationOptions<AgentschapZorgEnGezondheidFtpDumpConfiguration>(_configuration.GetSection(AgentschapZorgEnGezondheidFtpDumpConfiguration.Section)))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryAgentschapZorgEnGezondheidFtpDumpAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .SingleInstance();

            builder.RegisterType<Runner>()
                .SingleInstance();

            builder.Populate(_services);
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();
            return () => defaultServiceProvider.CreateScope().ServiceProvider;
        }
    }
}
