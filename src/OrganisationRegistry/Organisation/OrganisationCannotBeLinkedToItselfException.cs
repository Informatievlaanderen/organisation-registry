namespace OrganisationRegistry.Organisation
{
    public class OrganisationCannotBeLinkedToItselfException : DomainException
    {
        public OrganisationCannotBeLinkedToItselfException()
            : base("Een organisatie kan niet aan zichzelf gekoppeld worden.") { }
    }
}
