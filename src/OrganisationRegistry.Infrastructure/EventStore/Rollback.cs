namespace OrganisationRegistry.Infrastructure.EventStore;

using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using Messages;

public class Rollback : IEvent<Rollback>
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

    public Rollback(IEnumerable<IEvent> events)
        => Events = events.ToList();
}
