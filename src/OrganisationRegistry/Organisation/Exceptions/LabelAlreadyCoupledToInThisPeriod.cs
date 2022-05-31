namespace OrganisationRegistry.Organisation.Exceptions;

public class LabelAlreadyCoupledToInThisPeriod : DomainException
{
    public LabelAlreadyCoupledToInThisPeriod()
        : base("Dit label is in deze periode reeds gekoppeld aan de organisatie.") { }
}
