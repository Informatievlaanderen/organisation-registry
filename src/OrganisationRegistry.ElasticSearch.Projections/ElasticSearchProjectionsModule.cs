namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using Autofac;
    using System.Reflection;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using SqlServer;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Events;

    public class ElasticSearchProjectionsModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public ElasticSearchProjectionsModule(
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

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryElasticSearchProjectionsAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IReactionHandler<>))
                .SingleInstance();

            builder.RegisterType<OrganisationsRunner>()
                .SingleInstance();

            builder.RegisterType<PeopleRunner>()
                .SingleInstance();

            builder.RegisterType<BodyRunner>()
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
