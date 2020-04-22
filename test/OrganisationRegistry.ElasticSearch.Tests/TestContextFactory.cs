namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System.Data.Common;
    using Microsoft.EntityFrameworkCore;
    using SqlServer;
    using SqlServer.Infrastructure;

    public class TestContextFactory : IContextFactory
    {
        private readonly DbContextOptions<OrganisationRegistryContext> _contextOptions;

        public TestContextFactory(DbContextOptions<OrganisationRegistryContext> contextOptions)
        {
            _contextOptions = contextOptions;
        }

        public OrganisationRegistryContext CreateTransactional(DbConnection connection, DbTransaction transaction)
        {
            return new OrganisationRegistryContext(_contextOptions);
        }

        public OrganisationRegistryContext Create()
        {
            return new OrganisationRegistryContext(_contextOptions);
        }
    }
}
