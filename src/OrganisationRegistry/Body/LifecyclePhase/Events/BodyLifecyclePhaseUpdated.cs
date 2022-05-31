namespace OrganisationRegistry.Body.Events;

using System;
using LifecyclePhaseType;

public class BodyLifecyclePhaseUpdated : BaseEvent<BodyLifecyclePhaseUpdated>
{
    public Guid BodyId => Id;

    public Guid BodyLifecyclePhaseId { get; }
    public Guid LifecyclePhaseTypeId { get; }
    public Guid PreviousLifecyclePhaseTypeId { get; }
    public string LifecyclePhaseTypeName { get; }
    public string PreviousLifecyclePhaseTypeName { get; }
    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
    public LifecyclePhaseTypeIsRepresentativeFor PreviousLifecyclePhaseTypeIsRepresentativeFor { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }
    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public BodyLifecyclePhaseUpdated(
        Guid bodyId,
        Guid bodyLifecyclePhaseId,
        Guid lifecyclePhaseTypeId,
        string lifecyclePhaseTypeName,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousLifecyclePhaseTypeId,
        string previousLifecyclePhaseTypeName,
        LifecyclePhaseTypeIsRepresentativeFor previousLifecyclePhaseTypeIsRepresentativeFor,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodyLifecyclePhaseId = bodyLifecyclePhaseId;
        LifecyclePhaseTypeId = lifecyclePhaseTypeId;
        LifecyclePhaseTypeName = lifecyclePhaseTypeName;
        LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousLifecyclePhaseTypeId = previousLifecyclePhaseTypeId;
        PreviousLifecyclePhaseTypeName = previousLifecyclePhaseTypeName;
        PreviousLifecyclePhaseTypeIsRepresentativeFor = previousLifecyclePhaseTypeIsRepresentativeFor;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}
