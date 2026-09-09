namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationNameIsRequired : DomainException
{
    public OrganisationNameIsRequired()
        : base("Naam is verplicht.") { }
}
