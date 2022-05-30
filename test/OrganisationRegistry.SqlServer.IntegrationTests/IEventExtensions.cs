namespace OrganisationRegistry.SqlServer.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Infrastructure.Events;

public static class EventExtensions{

    public static IEnumerable<IEvent> Numbered(this IEnumerable<IEvent> source)
    {
        var ids = new Dictionary<Guid, int>();

        return source.Select(@event =>
        {
            if (!ids.ContainsKey(@event.Id))
                ids[@event.Id] = 0;

            ids[@event.Id] = ++ids[@event.Id];
            @event.Version = @event.Version != 0 ? @event.Version : ids[@event.Id];
            return @event;
        });
    }

    public static IEnumerable<IEvent> Stamped(this IEnumerable<IEvent> source)
    {
        return source.Select(@event =>
        {
            @event.Timestamp = DateTimeOffset.Now;
            return @event;
        });
    }

}