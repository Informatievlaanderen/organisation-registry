namespace OrganisationRegistry.Organisation.Commands;

public class CreateOrganisationFromKboNumber : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public KboNumber KboNumber { get; }

    public CreateOrganisationFromKboNumber(OrganisationId organisationId, KboNumber kboNumber)
    {
        Id = organisationId;
        KboNumber = kboNumber;
    }

    public static implicit operator CreateOrganisationFromKbo(CreateOrganisationFromKboNumber command)
        => new(
            command.OrganisationId,
            string.Empty,
            null,
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
