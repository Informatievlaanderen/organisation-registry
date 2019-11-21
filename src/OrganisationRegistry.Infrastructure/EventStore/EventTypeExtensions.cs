namespace OrganisationRegistry.Infrastructure.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class EventTypeExtensions
    {
        public static List<string> GetEventTypeNames(this IEnumerable<Type> eventTypes)
        {
            return eventTypes
                .Select(et => et.FullName)
                .ToList();
        }

        public static Type ToEventType(this string name)
        {
            return Type.GetType($"{name}, OrganisationRegistry");
        }
    }
}
