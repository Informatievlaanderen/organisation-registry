namespace OrganisationRegistry.Purpose.Commands;

public class CreatePurpose : BaseCommand<PurposeId>
{
    public PurposeId PurposeId => Id;

    public PurposeName Name { get; }

    public CreatePurpose(
        PurposeId purposeId,
        PurposeName name)
    {
        Id = purposeId;

        Name = name;
    }
}