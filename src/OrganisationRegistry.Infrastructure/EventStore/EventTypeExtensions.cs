namespace OrganisationRegistry.Infrastructure.EventStore;

using System;
using System.Collections.Generic;
using System.Linq;

public static class EventTypeExtensions
{
    public static List<string> GetEventTypeNames(this IEnumerable<Type> eventTypes)
        => eventTypes
            .Select(et => et.FullName ?? string.Empty)
            .ToList();

    public static Type ToEventType(this string name)
    {
        var typeName = $"{name}, OrganisationRegistry";
        var maybeType = Type.GetType(typeName);
        if (maybeType is { } type)
            return type;

        throw new CannotCreateType(typeName);
    }
}
