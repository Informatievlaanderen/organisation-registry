namespace OrganisationRegistry.Organisation.Exceptions;

public class CannotAddDuplicateContact : DomainException
{
    public CannotAddDuplicateContact()
        : base("Deze contactgegevens bestaan reeds voor de opgegeven periode")
    {
    }
}
