namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationBankAccountNotFound : DomainException
{
    public OrganisationBankAccountNotFound() : base("Het opgegeven bank account werd niet gevonden bij de organisatie")
    {
    }
}
