namespace OrganisationRegistry.Organisation.Commands;

public class CoupleOrganisationToKbo : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public KboNumber KboNumber { get; }

    public CoupleOrganisationToKbo(
        OrganisationId organisationId,
        KboNumber kboNumber)
    {
        Id = organisationId;

        KboNumber = kboNumber;
    }
}
