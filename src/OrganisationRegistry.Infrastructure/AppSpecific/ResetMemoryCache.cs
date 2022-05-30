namespace OrganisationRegistry.Infrastructure.AppSpecific;

using System;
using System.Collections.Generic;
using Events;
using Messages;
using System.Linq;

public class ResetMemoryCache : IEvent<ResetMemoryCache>
{
    protected Guid Id { get; set; }

    public int Version { get; set; }

    public DateTimeOffset Timestamp { get; set; }

    Guid IMessage.Id
    {
        get => Id;
        set => Id = value;
    }

    public List<IEvent> Events { get; }

    public ResetMemoryCache(IEnumerable<IEvent> events)
        => Events = events.ToList();
}