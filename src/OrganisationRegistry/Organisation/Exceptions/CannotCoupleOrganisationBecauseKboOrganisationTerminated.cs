namespace OrganisationRegistry.Organisation.Exceptions;

public class CannotCoupleOrganisationBecauseKboOrganisationTerminated : DomainException
{
    public CannotCoupleOrganisationBecauseKboOrganisationTerminated()
        : base("Deze organisatie is stopgezet in de KBO. Enkel koppelingen aan een actief KBO nummer zijn toegelaten.") { }
}