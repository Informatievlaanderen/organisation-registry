namespace OrganisationRegistry.ContactType.Events;

using System;

public class ContactTypeCreated : BaseEvent<ContactTypeCreated>
{
    public Guid ContactTypeId => Id;

    public string Name { get; }
    public string? Regex { get; }
    public string? Example { get; }

    public ContactTypeCreated(
        Guid contactTypeId,
        string name,
        string regex,
        string example)
    {
        Id = contactTypeId;
        Name = name;
        Regex = regex;
        Example = example;
    }
}
