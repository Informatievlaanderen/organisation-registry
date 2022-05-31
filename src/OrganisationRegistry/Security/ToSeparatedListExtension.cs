namespace OrganisationRegistry.Security;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ToSeparatedListExtension
{
    public static string ToSeparatedList<T>(
        this IEnumerable<T> list,
        string separator,
        Func<T, string> valueFunc)
        => string.Join(separator, list.Select(valueFunc));
}
