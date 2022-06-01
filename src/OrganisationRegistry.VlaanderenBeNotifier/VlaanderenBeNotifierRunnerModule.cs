namespace OrganisationRegistry.VlaanderenBeNotifier;

using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using ElasticSearch;
using Infrastructure;
using Infrastructure.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using SqlServer;
using System.Reflection;
using Infrastructure.Authorization;
using Microsoft.Extensions.Logging;

public class VlaanderenBeNotifierRunnerModule : Autofac.Module
{
    private readonly IConfiguration _configuration;
    private readonly IServiceCollection _services;
    private readonly ILoggerFactory _loggerFactory;

    public VlaanderenBeNotifierRunnerModule(
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
        builder.RegisterModule(new OrganisationRegistryModule());
        builder.RegisterModule(new ElasticSearchModule(_configuration, _services));
        builder.RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory));
        builder.RegisterModule(new Schema.VlaanderenBeNotifierModule(_configuration, _services, _loggerFactory));
        builder.RegisterModule(new VlaanderenBeNotifierModule(_configuration, _services));

        builder.RegisterAssemblyTypes(typeof(OrganisationRegistryVlaanderenBeNotifierAssemblyTokenClass).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(IEventHandler<>))
            .SingleInstance();

        builder.RegisterType<SendGridMailer>()
            .As<IMailer>()
            .SingleInstance();

        builder.RegisterType<NotImplementedSecurityService>()
            .As<ISecurityService>()
            .InstancePerLifetimeScope();

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
