namespace OrganisationRegistry.SqlServer
{
    using System.Reflection;
    using Autofac;
    using Configuration;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using ProjectionState;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;

    public class SqlServerModule : Autofac.Module
    {
        public SqlServerModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            var sqlConfiguration = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>();

            services
                .AddDbContext<OrganisationRegistryContext>(options => options
                    .UseSqlServer(
                        connectionString: sqlConfiguration.ConnectionString,
                        sqlServerOptionsAction: x => x
                            .MigrationsAssembly("OrganisationRegistry.SqlServer")
                            .MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")));

            services.Configure<SqlServerConfiguration>(configuration.GetSection(SqlServerConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectionStates>()
                .As<IProjectionStates>()
                .SingleInstance();

            builder.RegisterType<MemoryCaches>()
                .As<MemoryCaches>()
                .As<IMemoryCaches>()
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistrySqlServerAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IEventHandler<>))
                .SingleInstance();

            builder.RegisterAssemblyTypes(typeof(OrganisationRegistrySqlServerAssemblyTokenClass).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(IReactionHandler<>))
                .SingleInstance();
        }
    }
}
