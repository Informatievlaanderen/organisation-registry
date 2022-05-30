namespace OrganisationRegistry.LifecyclePhaseType.Events;

using System;

public class LifecyclePhaseTypeCreated : BaseEvent<LifecyclePhaseTypeCreated>
{
    public Guid LifecyclePhaseTypeId => Id;

    public string Name { get; }
    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
    public LifecyclePhaseTypeStatus Status { get; }

    public LifecyclePhaseTypeCreated(
        Guid lifecyclePhaseTypeId,
        string name,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeStatus status)
    {
        Id = lifecyclePhaseTypeId;
        Name = name;
        LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
        Status = status;
    }
}