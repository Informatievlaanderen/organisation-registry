namespace OrganisationRegistry.Infrastructure
{
    using System;
    using Autofac;
    using Autofac.Core;
    using Bus;
    using Commands;
    using Config;
    using Configuration;
    using Domain;
    using Events;
    using EventStore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class InfrastructureModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly Func<IComponentContext, Func<IServiceProvider>> _scopedServiceProvider;

        public InfrastructureModule(
            IConfiguration configuration,
            Func<IComponentContext, Func<IServiceProvider>> scopedServiceProvider,
            IServiceCollection services)
        {
            _configuration = configuration;
            _scopedServiceProvider = scopedServiceProvider;

            services.Configure<InfrastructureConfiguration>(_configuration.GetSection(InfrastructureConfiguration.Section));
            services.Configure<TogglesConfiguration>(_configuration.GetSection(TogglesConfiguration.Section));
            services.Configure<OpenIdConnectConfiguration>(_configuration.GetSection(OpenIdConnectConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<LoggerFactory>()
                .As<ILoggerFactory>()
                .SingleInstance();

            // This is the same as services.AddLogging();
            builder.RegisterGeneric(typeof(Logger<>))
                .As(typeof(ILogger<>))
                .SingleInstance();

            builder.RegisterInstance(_configuration)
                .As<IConfiguration>()
                .SingleInstance();

            builder.RegisterType<InProcessBus>()
                .As<InProcessBus>()
                .As<ICommandSender>()
                .As<IEventPublisher>()
                .As<IHandlerRegistrar>()
                .SingleInstance();

            builder.RegisterType<Session>()
                .As<ISession>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SqlServerEventStore>()
                .As<SqlServerEventStore>()
                .As<IEventStore>()
                .SingleInstance();

            builder.RegisterType<Repository>()
                .As<IRepository>()
                .SingleInstance();

            builder.RegisterType<BusRegistrar>()
                .WithParameter(
                    new ResolvedParameter(
                        (info, context) => info.Name == "requestScopedServiceProvider",
                        (info, context) => _scopedServiceProvider(context)))
                .SingleInstance();

            builder.RegisterType<ExternalIpFetcher>()
                .As<IExternalIpFetcher>()
                .SingleInstance();
        }
    }
}
