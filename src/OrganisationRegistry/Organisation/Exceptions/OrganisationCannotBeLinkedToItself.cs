namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationCannotBeLinkedToItself : DomainException
{
    public OrganisationCannotBeLinkedToItself()
        : base("Een organisatie kan niet aan zichzelf gekoppeld worden.") { }
}