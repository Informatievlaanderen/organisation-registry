// ReSharper disable UnusedParameter.Local
namespace OrganisationRegistry.Capacity;

using Events;
using Infrastructure.Domain;

public class Capacity : AggregateRoot
{
    public string Name { get; private set; } = null!;
    public bool IsRemoved { get; private set; }

    private Capacity() { }

    public Capacity(CapacityId id, string name)
    {
        ApplyChange(new CapacityCreated(id, name));
    }

    public void Update(string name)
    {
        var @event = new CapacityUpdated(Id, name, Name);
        ApplyChange(@event);
    }

    public void Remove()
    {
        if (IsRemoved) return;

        ApplyChange(new CapacityRemoved(Id));
    }

    private void Apply(CapacityCreated @event)
    {
        Id = @event.CapacityId;
        Name = @event.Name;
    }

    private void Apply(CapacityUpdated @event)
    {
        Name = @event.Name;
    }

    private void Apply(CapacityRemoved @event)
    {
        IsRemoved = true;
    }
}
