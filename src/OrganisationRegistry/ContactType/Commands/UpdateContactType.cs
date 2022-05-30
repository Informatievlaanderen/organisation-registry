namespace OrganisationRegistry.ContactType.Commands;

public class UpdateContactType : BaseCommand<ContactTypeId>
{
    public ContactTypeId ContactTypeId => Id;

    public string Name { get; }

    public UpdateContactType(
        ContactTypeId contactTypeId,
        string name)
    {
        Id = contactTypeId;
        Name = name;
    }
}