namespace OrganisationRegistry.Projections.Reporting
{
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Configuration;
    using ElasticSearch;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;
    using Runners;
    using SqlServer;
    using System;
    using System.Reflection;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;

    public class ReportingRunnerModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;

        public ReportingRunnerModule(
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
            builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
            builder.RegisterModule(new ElasticSearchModule(_configuration, _services));
            builder.RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory));

            builder.RegisterInstance<IConfigureOptions<ReportingRunnerConfiguration>>(
                    new ConfigureFromConfigurationOptions<ReportingRunnerConfiguration>(_configuration.GetSection(ReportingRunnerConfiguration.Section)))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryReportingRunnerAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryReportingRunnerAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IReactionHandler<>))
                .SingleInstance();

            builder.RegisterType<GenderRatioRunner>()
                .SingleInstance();

            builder.RegisterType<NotImplementedSecurityService>()
                .As<ISecurityService>()
                .InstancePerLifetimeScope();

            builder.Populate(_services);
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();
            return () => defaultServiceProvider.CreateScope().ServiceProvider;
        }
    }
}
