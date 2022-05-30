namespace OrganisationRegistry.Function;

using Events;
using Infrastructure.Domain;

public class FunctionType : AggregateRoot
{
    public string Name { get; private set; }

    private FunctionType()
    {
        Name = string.Empty;
    }

    public FunctionType(FunctionTypeId id, string name)
    {
        Name = string.Empty;

        ApplyChange(new FunctionCreated(id, name));
    }

    public void Update(string name)
    {
        var @event = new FunctionUpdated(Id, name, Name);
        ApplyChange(@event);
    }

    private void Apply(FunctionCreated @event)
    {
        Id = @event.FunctionId;
        Name = @event.Name;
    }

    private void Apply(FunctionUpdated @event)
    {
        Name = @event.Name;
    }
}