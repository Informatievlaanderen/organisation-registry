namespace OrganisationRegistry.Organisation.Exceptions;

public class BankAccountNumberAlreadyCoupledToInThisPeriod : DomainException
{
    public BankAccountNumberAlreadyCoupledToInThisPeriod()
        : base("Dit bankrekeningnummer is in deze periode reeds gekoppeld aan de organisatie.") { }
}
