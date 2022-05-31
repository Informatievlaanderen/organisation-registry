namespace OrganisationRegistry.ContactType.Events;

using System;

public class ContactTypeUpdated : BaseEvent<ContactTypeUpdated>
{
    public Guid ContactTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public ContactTypeUpdated(
        Guid contactTypeId,
        string name,
        string previousName)
    {
        Id = contactTypeId;

        Name = name;
        PreviousName = previousName;
    }
}
