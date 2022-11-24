namespace OrganisationRegistry.Tests.Shared;

using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SqlServer;
using OrganisationRegistry.SqlServer.Infrastructure;

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
