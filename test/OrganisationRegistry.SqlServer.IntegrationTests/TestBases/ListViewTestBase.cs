﻿using Microsoft.Extensions.Logging;

namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.IO;
    using System.Reflection;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using OnProjections;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;

    public abstract class ListViewTestBase
    {
        private readonly SqlServerFixture _fixture;
        private readonly SqlConnection _sqlConnection;
        private readonly SqlTransaction _transaction;
        private readonly ServiceCollection _serviceCollection;
        private InProcessBus _inProcessBus;
        private readonly ServiceProvider _serviceProvider;
        private SqlServerEventStore _sqlServerEventStore;
        private List<Func<IEventStore, object>> _handlers;
        private OrganisationRegistryContext _context;
        public DbContextOptions<OrganisationRegistryContext> ContextOptions { get; }
        protected OrganisationRegistryContext Context => new OrganisationRegistryContext(ContextOptions);

        protected ListViewTestBase()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location).Parent.Parent.Parent.FullName);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
                .Build();

            ContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    $"org-es-test-{Guid.NewGuid()}",
                    builder => { }).Options;

            _context = new OrganisationRegistryContext(ContextOptions);

            RunProgram(configuration);
        }

        private void RunProgram(IConfiguration configuration)
        {
            var services = new ServiceCollection();

            var app = ConfigureServices(services, configuration, ContextOptions);
            UseOrganisationRegistryEventSourcing(app);

            _inProcessBus = app.GetService<InProcessBus>();

        }

        private static IServiceProvider ConfigureServices(IServiceCollection services,
            IConfiguration configuration,
            DbContextOptions<OrganisationRegistryContext> dbContextOptions)
        {
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();

            var builder = new ContainerBuilder();
            builder.RegisterModule(new TestRunnerModule(configuration, services, new NullLoggerFactory(), dbContextOptions));
            return new AutofacServiceProvider(builder.Build());
        }

        private static void UseOrganisationRegistryEventSourcing(IServiceProvider app)
        {
            var registrar = app.GetService<BusRegistrar>();

            registrar.RegisterEventHandlers(typeof(MemoryCachesMaintainer));
            registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));
        }


        public void HandleEvents(params IEvent[] envelopes)
        {
            foreach (var envelope in envelopes)
            {
                _inProcessBus.Publish(Mock.Of<DbConnection>(), Mock.Of<DbTransaction>(), (dynamic)envelope.ToEnvelope());
            }
        }
    }

    public class TestRunnerModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;
        private readonly ILoggerFactory _loggerFactory;
        private readonly DbContextOptions<OrganisationRegistryContext> _dbContextOptions;

        public TestRunnerModule(IConfiguration configuration,
            IServiceCollection services,
            ILoggerFactory loggerFactory,
            DbContextOptions<OrganisationRegistryContext> dbContextOptions)
        {
            _configuration = configuration;
            _services = services;
            _loggerFactory = loggerFactory;
            _dbContextOptions = dbContextOptions;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule(_configuration, ProvideScopedServiceProvider, _services));
            builder.RegisterModule(new OrganisationRegistryModule());
            builder.RegisterModule(new SqlServerModule(_configuration, _services, _loggerFactory));

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistrySqlServerAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .SingleInstance();

            builder.RegisterInstance<Func<DbConnection, DbTransaction, OrganisationRegistryContext>>(
                (dbConn, dbTrans) => new OrganisationRegistryContext(_dbContextOptions));

            builder.Populate(_services);
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();
            return () => defaultServiceProvider.CreateScope().ServiceProvider;
        }
    }
}
