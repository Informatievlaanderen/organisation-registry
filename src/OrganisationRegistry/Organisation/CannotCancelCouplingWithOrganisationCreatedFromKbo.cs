namespace OrganisationRegistry.Organisation
{
    public class CannotCancelCouplingWithOrganisationCreatedFromKbo: DomainException
    {
        public CannotCancelCouplingWithOrganisationCreatedFromKbo()
            : base("Het ongedaan maken van een koppeling met de KBO is momenteel niet ondersteund voor organisaties die rechtstreeks gecreëerd werden vanuit een koppeling met de KBO.") { }
    }
}
