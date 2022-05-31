namespace OrganisationRegistry.LifecyclePhaseType.Commands;

public class UpdateLifecyclePhaseType : BaseCommand<LifecyclePhaseTypeId>
{
    public LifecyclePhaseTypeId LifecyclePhaseTypeId => Id;

    public LifecyclePhaseTypeName Name { get; }
    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get; }
    public LifecyclePhaseTypeStatus Status { get; }

    public UpdateLifecyclePhaseType(
        LifecyclePhaseTypeId lifecyclePhaseTypeId,
        LifecyclePhaseTypeName name,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeStatus status)
    {
        Id = lifecyclePhaseTypeId;
        Name = name;
        LifecyclePhaseTypeIsRepresentativeFor = lifecyclePhaseTypeIsRepresentativeFor;
        Status = status;
    }
}
