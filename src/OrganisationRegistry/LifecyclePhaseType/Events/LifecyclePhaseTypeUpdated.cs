namespace OrganisationRegistry.LifecyclePhaseType.Events;

using System;

public class LifecyclePhaseTypeUpdated : BaseEvent<LifecyclePhaseTypeUpdated>
{
    public Guid LifecyclePhaseTypeId => Id;

    public string Name { get; }
    public string PreviousName { get; }

    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
    public LifecyclePhaseTypeIsRepresentativeFor PreviousLifecyclePhaseTypeIsRepresentativeFor { get; }

    public LifecyclePhaseTypeStatus Status { get; }
    public LifecyclePhaseTypeStatus PreviousStatus { get; }

    public LifecyclePhaseTypeUpdated(
        Guid lifecyclePhaseTypeId,
        string name,
        string previousName,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeIsRepresentativeFor previousLifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeStatus status,
        LifecyclePhaseTypeStatus previousStatus)
    {
        Id = lifecyclePhaseTypeId;

        Name = name;
        PreviousName = previousName;

        LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
        PreviousLifecyclePhaseTypeIsRepresentativeFor = previousLifecyclePhaseTypeIsRepresentativeFor;

        Status = status;
        PreviousStatus = previousStatus;
    }
}
