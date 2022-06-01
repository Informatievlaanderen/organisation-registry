namespace OrganisationRegistry.Infrastructure.Domain;

using System;
using System.Collections.Generic;
using Events;
using Exception;
using Infrastructure;

public abstract class AggregateRoot
{
    private readonly List<IEvent> _changes = new();

    public Guid Id { get; protected set; }

    public int Version { get; protected set; }

    public IEnumerable<IEvent> GetUncommittedChanges()
    {
        lock (_changes)
            return _changes.ToArray();
    }

    public IEnumerable<IEvent> FlushUncommitedChanges()
    {
        lock (_changes)
        {
            var changes = _changes.ToArray();
            var i = 0;
            foreach (var @event in changes)
            {
                if (@event.Id == Guid.Empty && Id == Guid.Empty)
                    throw new AggregateOrEventMissingIdException(GetType(), @event.GetType());

                if (@event.Id == Guid.Empty)
                    @event.Id = Id;

                i++;
                @event.Version = Version + i;
                @event.Timestamp = DateTimeOffset.UtcNow;
            }

            Version += _changes.Count;
            _changes.Clear();
            return changes;
        }
    }

    public void LoadFromHistory(IEnumerable<IEvent> history)
    {
        foreach (var e in history)
        {
            if (e.Version != Version + 1)
                throw new EventsOutOfOrderException(e.Id);

            ApplyChange(e, false);
        }
    }

    protected void ApplyChange(IEvent @event)
        => ApplyChange(@event, true);

    private void ApplyChange(IEvent @event, bool isNew)
    {
        lock (_changes)
        {
            this.AsDynamic().Apply(@event);

            if (isNew)
            {
                _changes.Add(@event);
            }
            else
            {
                if (Id != Guid.Empty)
                    Id = @event.Id;

                Version++;
            }
        }
    }

    protected bool Equals(AggregateRoot other)
        => Id.Equals(other.Id);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((AggregateRoot)obj);
    }

    public override int GetHashCode()
        // ReSharper disable once NonReadonlyMemberInGetHashCode
        => Id.GetHashCode();
}
