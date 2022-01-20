namespace OrganisationRegistry.Organisation.Exceptions
{
    public class CannotCreateOrganisationBecauseKboOrganisationTerminated : DomainException
    {
        public CannotCreateOrganisationBecauseKboOrganisationTerminated()
            : base("Deze organisatie is stopgezet in de KBO. Het is niet toegelaten een organisatie te creëren met een inactief KBO nummer.") { }
    }
}
