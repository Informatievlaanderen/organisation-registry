namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class MemoryCacheExtensions
    {
        public static Task<Dictionary<TKey, TValue>> BuildMemoryCache<T, TKey, TValue>(
            this IQueryable<T> queryable,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector) where T : class
        {
            return queryable.AsNoTracking().ToDictionaryAsync(keySelector, valueSelector);
        }

        public static void UpdateMemoryCache<T>(this Dictionary<Guid, T> dictionary, Guid key, T value)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
    }
}
