namespace OrganisationRegistry.ElasticSearch.Tests
{
    using System.Threading.Tasks;
    using Projections.People.Cache;
    using Projections.People.Handlers;
    using SqlServer.Infrastructure;

    public class PersonHandlerCacheStub : PersonHandlerCache
    {
        public override Task ClearCache(OrganisationRegistryContext context)
            => Task.CompletedTask;
    }
}
