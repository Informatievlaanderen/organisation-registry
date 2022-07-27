namespace OrganisationRegistry.ContactType.Events;

using System;

public class ContactTypeUpdated : BaseEvent<ContactTypeUpdated>
{
    public Guid ContactTypeId => Id;

    public string Name { get; }
    public string? Regex { get; }
    public string? Example { get; }
    public string PreviousName { get; }
    public string? PreviousRegex { get; }
    public string? PreviousExample { get; }

    public ContactTypeUpdated(
        Guid contactTypeId,
        string name,
        string regex,
        string example,
        string previousName,
        string previousRegex,
        string previousExample)
    {
        Id = contactTypeId;

        Name = name;
        Regex = regex;
        Example = example;
        PreviousName = previousName;
        PreviousRegex = previousRegex;
        PreviousExample = previousExample;
    }
}
