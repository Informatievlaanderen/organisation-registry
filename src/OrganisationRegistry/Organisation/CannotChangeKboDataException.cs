namespace OrganisationRegistry.Organisation
{
    public class CannotChangeKboDataException: DomainException
    {
        public CannotChangeKboDataException()
            : base("Deze data kan niet gewijzigd worden.") { }
    }
}
