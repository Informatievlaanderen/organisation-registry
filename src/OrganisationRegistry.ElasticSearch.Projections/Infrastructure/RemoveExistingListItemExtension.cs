namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using System;
using System.Linq;
using System.Collections.Generic;

public static class RemoveExistingListItemExtension
{
    public static void RemoveExistingListItems<T>(this IList<T> list, Func<T, bool> idFunc)
    {
        var items = list.Where(idFunc).ToList();
        foreach (var item in items)
            list.Remove(item);
    }

    public static void RemoveAndAdd<T>(this IList<T> list, Func<T, bool> idToRemoveFunc, T itemToAdd)
    {
        var items = list.Where(idToRemoveFunc).ToList();
        foreach (var item in items)
            list.Remove(item);

        list.Add(itemToAdd);
    }
}
