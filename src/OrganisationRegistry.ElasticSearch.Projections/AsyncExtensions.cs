namespace OrganisationRegistry.ElasticSearch.Projections;

using System.Threading.Tasks;

public static class AsyncExtensions
{
    public static Task<T> ToAsyncResult<T>(this T result)
        => Task.FromResult(result);
}
