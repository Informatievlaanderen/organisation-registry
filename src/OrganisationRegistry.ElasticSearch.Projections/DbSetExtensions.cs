namespace OrganisationRegistry.ElasticSearch.Projections;

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

public static class DbSetExtensions
{
    public static async Task<T> FindRequiredAsync<T>(this DbSet<T> dbSet, params object?[]? keyValues) where T : class
    {
        var maybeFound = await dbSet.FindAsync(keyValues);
        if (maybeFound is { } found)
            return found;

        throw new NullReferenceException($"key {ArrayToString(keyValues)} was not found in {typeof(T).Name}");
    }

    private static string ArrayToString(object?[]? maybeArray)
        => maybeArray is not { } array ? string.Empty : string.Join(',', array);
}
