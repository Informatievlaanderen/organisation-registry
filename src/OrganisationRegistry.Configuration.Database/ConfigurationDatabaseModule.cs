namespace OrganisationRegistry.Configuration.Database
{
    using Autofac;
    using Configuration;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class ConfigurationDatabaseModule : Module
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _services;

        public ConfigurationDatabaseModule(
            IConfiguration configuration,
            IServiceCollection services)
        {
            _configuration = configuration;
            _services = services;

            var sqlConfiguration = configuration.GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>();

            services.AddDbContext<ConfigurationContext>(options => options.UseSqlServer(
                sqlConfiguration.ConnectionString,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry")));

            _services.Configure<ConfigurationDatabaseConfiguration>(
                _configuration.GetSection(ConfigurationDatabaseConfiguration.Section));
        }

        protected override void Load(ContainerBuilder builder)
        {
        }
    }
}
