namespace OrganisationRegistry.Configuration.Database
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    public class ConfigurationContext : DbContext
    {
        public DbSet<ConfigurationValue> Configuration { get; set; }

        // This needs to be DbContextOptions<T> for Autofac!
        public ConfigurationContext(DbContextOptions<ConfigurationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
    }
}
