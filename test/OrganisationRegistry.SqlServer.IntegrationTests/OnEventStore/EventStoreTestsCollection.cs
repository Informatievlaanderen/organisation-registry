namespace OrganisationRegistry.SqlServer.IntegrationTests.OnEventStore
{
    using Xunit;

    [CollectionDefinition(Name)]
    public class EventStoreTestsCollection : ICollectionFixture<EventStoreSqlServerFixture>
    {
        public const string Name = "Eventstore Sql Tests Collection";
    }
}
