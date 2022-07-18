namespace OrganisationRegistry.Organisation.Exceptions;

public class OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType:DomainException
{
    public OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType()
        : base("De opgegeven oganisatie classificatie behoord niet tot het opgegeven organisatie classificatie type")
    {
    }
}
