namespace OrganisationRegistry.ContactType.Events;

using System;

public class ContactTypeCreated : BaseEvent<ContactTypeCreated>
{
    public Guid ContactTypeId => Id;

    public string Name { get; }

    public ContactTypeCreated(
        Guid contactTypeId,
        string name)
    {
        Id = contactTypeId;
        Name = name;
    }
}