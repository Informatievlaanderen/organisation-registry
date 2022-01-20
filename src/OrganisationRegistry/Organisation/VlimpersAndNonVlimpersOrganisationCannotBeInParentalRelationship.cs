namespace OrganisationRegistry.Organisation
{
    public class VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship : DomainException
    {
        public VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship()
            : base("Organisaties niet onder Vlimpersbeheer kunnen niet worden gekoppeld aan organisaties onder Vlimpersbeheer.") { }
    }
}
