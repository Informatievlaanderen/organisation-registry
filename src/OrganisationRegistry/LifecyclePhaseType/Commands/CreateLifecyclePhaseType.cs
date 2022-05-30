namespace OrganisationRegistry.LifecyclePhaseType.Commands;

public class CreateLifecyclePhaseType : BaseCommand<LifecyclePhaseTypeId>
{
    public LifecyclePhaseTypeId LifecyclePhaseTypeId => Id;

    public LifecyclePhaseTypeName Name { get; }
    public LifecyclePhaseTypeIsRepresentativeFor LifecyclePhaseTypeIsRepresentativeFor { get;  }
    public LifecyclePhaseTypeStatus Status { get; }

    public CreateLifecyclePhaseType(
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