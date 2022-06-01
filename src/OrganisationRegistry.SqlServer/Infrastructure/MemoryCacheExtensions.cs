namespace OrganisationRegistry.SqlServer.Infrastructure;

using System;
using System.Collections.Generic;

public static class MemoryCacheExtensions
{
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
