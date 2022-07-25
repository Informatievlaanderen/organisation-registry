namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationBankAccountNotFound : DomainException
{
    public OrganisationBankAccountNotFound() : base("De opgegeven bankrekening werd niet gevonden bij de organisatie")
    {
    }
}
