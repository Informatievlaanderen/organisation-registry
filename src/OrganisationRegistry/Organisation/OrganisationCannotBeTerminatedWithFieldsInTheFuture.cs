namespace OrganisationRegistry.Organisation
{
    public class OrganisationCannotBeTerminatedWithFieldsInTheFuture: DomainException
    {
        public OrganisationCannotBeTerminatedWithFieldsInTheFuture()
            : base("De organisatie bevat velden die volledig in de toekomst liggen, en kan niet afgesloten worden.") { }
    }
}
