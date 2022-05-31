namespace OrganisationRegistry.Body.Events;

using System;

public class BodyFormalValidityChanged : BaseEvent<BodyFormalValidityChanged>
{
    public Guid BodyId => Id;

    public DateTime? FormalValidFrom { get; }
    public DateTime? FormalValidTo { get; }

    public DateTime? PreviouslyFormallyValidFrom { get; }
    public DateTime? PreviouslyFormallyValidTo { get; }

    public BodyFormalValidityChanged(
        Guid bodyId,
        DateTime? formalValidFrom,
        DateTime? formalValidTo,
        DateTime? previouslyFormallyValidFrom,
        DateTime? previouslyFormallyValidTo)
    {
        Id = bodyId;

        FormalValidFrom = formalValidFrom;
        FormalValidTo = formalValidTo;

        PreviouslyFormallyValidFrom = previouslyFormallyValidFrom;
        PreviouslyFormallyValidTo = previouslyFormallyValidTo;
    }
}
