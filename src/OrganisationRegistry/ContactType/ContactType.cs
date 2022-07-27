namespace OrganisationRegistry.ContactType;

using System.Text.RegularExpressions;
using Events;
using Infrastructure.Domain;

public class ContactType : AggregateRoot
{
    public string Name { get; private set; }
    public Regex Regex { get; private set; }
    public string Example { get; private set; }

    private ContactType()
    {
        Name = string.Empty;
        Regex = new Regex(".*");
        Example = string.Empty;
    }

    public ContactType(ContactTypeId id, string name, Regex regex, string example) : this()
    {
        ThrowIfExampleDoesNotMatchRegex(regex, example);

        ApplyChange(new ContactTypeCreated(id, name, regex.ToString(), example));
    }

    public void Update(string name, Regex regex, string example)
    {
        ThrowIfExampleDoesNotMatchRegex(regex, example);

        var @event = new ContactTypeUpdated(Id, name, regex.ToString(), example, Name, Regex.ToString(), Example);
        ApplyChange(@event);
    }

    private static void ThrowIfExampleDoesNotMatchRegex(Regex regex, string example)
    {
        if (!regex.IsMatch(example))
            throw new ExampleDoesNotMatchRegex();
    }

    public void ThrowIfInvalidValue(string value)
    {
        if (!Regex.IsMatch(value))
            throw new ValueDoesNotMatchRegex(Name, Example);
    }

    private void Apply(ContactTypeCreated @event)
    {
        Id = @event.ContactTypeId;
        Name = @event.Name;
        Regex = new Regex(@event.Regex ?? ".*");
        Example = @event.Example ?? "";
    }

    private void Apply(ContactTypeUpdated @event)
    {
        Name = @event.Name;
        Regex = new Regex(@event.Regex ?? ".*");
        Example = @event.Example ?? "";
    }
}
