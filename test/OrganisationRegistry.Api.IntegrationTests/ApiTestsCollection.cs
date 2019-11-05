namespace OrganisationRegistry.Api.IntegrationTests
{
    using Xunit;
    [CollectionDefinition(Name)]
    public class ApiTestsCollection : ICollectionFixture<ApiFixture>
    {
        public const string Name = "Api Tests Collection";
    }
}
