namespace OrganisationRegistry.Purpose.Commands;

public class UpdatePurpose : BaseCommand<PurposeId>
{
    public PurposeId PurposeId => Id;

    public PurposeName Name { get; }

    public UpdatePurpose(
        PurposeId purposeId,
        PurposeName name)
    {
        Id = purposeId;

        Name = name;
    }
}