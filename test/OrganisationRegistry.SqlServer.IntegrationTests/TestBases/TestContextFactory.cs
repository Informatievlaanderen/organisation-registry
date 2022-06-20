namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases;

using System.Data.Common;
using Infrastructure;
using Microsoft.EntityFrameworkCore;

public class TestContextFactory : IContextFactory
{
    private readonly DbContextOptions<OrganisationRegistryContext> _contextOptions;

    public TestContextFactory(DbContextOptions<OrganisationRegistryContext> contextOptions)
    {
        _contextOptions = contextOptions;
    }

    public OrganisationRegistryContext CreateTransactional(DbConnection connection, DbTransaction transaction)
        => new(_contextOptions);

    public OrganisationRegistryContext Create()
        => new(_contextOptions);
}
