namespace OrganisationRegistry.Body.Events;

using System;
using LifecyclePhaseType;

public class BodyLifecyclePhaseAdded : BaseEvent<BodyLifecyclePhaseAdded>
{
    public Guid BodyId => Id;

    public Guid BodyLifecyclePhaseId { get; }
    public Guid LifecyclePhaseTypeId { get; }
    public string LifecyclePhaseTypeName { get; }
    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public BodyLifecyclePhaseAdded(
        Guid bodyId,
        Guid bodyLifecyclePhaseId,
        Guid lifecyclePhaseTypeId,
        string lifecyclePhaseTypeName,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = bodyId;

        BodyLifecyclePhaseId = bodyLifecyclePhaseId;
        LifecyclePhaseTypeId = lifecyclePhaseTypeId;
        LifecyclePhaseTypeName = lifecyclePhaseTypeName;
        LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}