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

    public class AgentschapZorgEnGezondheidFtpDumpModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public AgentschapZorgEnGezondheidFtpDumpModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
            builder.RegisterModule(new OrganisationRegistryModule());
            builder.RegisterModule(new ElasticSearchModule(_configuration, _services));
            builder.RegisterModule(new SqlServerModule(_configuration, _services));

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
