namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections
{
    using Xunit;

    [CollectionDefinition(Name)]
    public class SqlServerTestsCollection : ICollectionFixture<SqlServerFixture>
    {
        public const string Name = "Sql Tests Collection";
    }
}
