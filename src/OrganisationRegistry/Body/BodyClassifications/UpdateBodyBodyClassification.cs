namespace OrganisationRegistry.Body;

using System;
using BodyClassification;
using BodyClassificationType;

public class UpdateBodyBodyClassification : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public Guid BodyBodyClassificationId { get; }
    public BodyClassificationTypeId BodyClassificationTypeId { get; }
    public BodyClassificationId BodyClassificationId { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateBodyBodyClassification(
        Guid bodyBodyClassificationId,
        BodyId bodyId,
        BodyClassificationTypeId bodyClassificationTypeId,
        BodyClassificationId bodyClassificationId,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = bodyId;

        BodyBodyClassificationId = bodyBodyClassificationId;
        BodyClassificationTypeId = bodyClassificationTypeId;
        BodyClassificationId = bodyClassificationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
