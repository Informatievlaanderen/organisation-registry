namespace OrganisationRegistry.Organisation.Commands;

public class CreateOrganisationFromKboNumber : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId
        => Id;

    public string OvoNumber { get; }
    public KboNumber KboNumber { get; }

    public CreateOrganisationFromKboNumber(OrganisationId organisationId, KboNumber kboNumber, string ovoNumber)
    {
        Id = organisationId;
        KboNumber = kboNumber;
        OvoNumber = ovoNumber;
    }

    public static implicit operator CreateOrganisationFromKbo(CreateOrganisationFromKboNumber command)
        => new(
            command.OrganisationId,
            string.Empty,
            command.OvoNumber,
            null,
            Article.None,
            null,
            null,
            null,
            false,
            new ValidFrom(),
            new ValidTo(),
            command.KboNumber,
            new ValidFrom(),
            new ValidTo()
        );
}
