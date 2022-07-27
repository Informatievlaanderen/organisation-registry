namespace OrganisationRegistry.ContactType.Commands;

using System.Text.RegularExpressions;

public class UpdateContactType : BaseCommand<ContactTypeId>
{
    public ContactTypeId ContactTypeId => Id;

    public string Name { get; }

    public Regex Regex { get; }

    public string Example { get; }

    public UpdateContactType(
        ContactTypeId contactTypeId,
        string name,
        Regex regex,
        string example)
    {
        Id = contactTypeId;
        Name = name;
        Regex = regex;
        Example = example;
    }
}
