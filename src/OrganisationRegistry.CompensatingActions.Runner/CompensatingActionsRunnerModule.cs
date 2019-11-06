namespace OrganisationRegistry.CompensatingActions.Runner
{
    using System;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SqlServer;
    using Infrastructure;
    using ElasticSearch;

    public class CompensatingActionsRunnerModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public CompensatingActionsRunnerModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
            builder.RegisterModule(new ElasticSearchModule(_configuration, _services));
            builder.RegisterModule(new SqlServerModule(_configuration, _services));

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
