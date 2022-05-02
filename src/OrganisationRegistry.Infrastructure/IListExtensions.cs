namespace OrganisationRegistry.Infrastructure;

using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once InconsistentNaming
public static class IListExtensions
{
    public static IEnumerable<TSource> Except<TSource>(this IList<TSource> source, Func<TSource, bool> predicate)
        => source.Except(new[]
        {
            source.Single(predicate)
        });
}
