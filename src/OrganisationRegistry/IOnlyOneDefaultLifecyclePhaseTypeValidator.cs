namespace OrganisationRegistry;

using LifecyclePhaseType;

public interface IOnlyOneDefaultLifecyclePhaseTypeValidator
{
    bool ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeStatus lifecyclePhaseTypeStatus);

    bool ViolatesOnlyOneDefaultLifecyclePhaseTypeConstraint(
        LifecyclePhaseTypeId lifecyclePhaseTypeId,
        LifecyclePhaseTypeIsRepresentativeFor lifecyclePhaseTypeIsRepresentativeFor,
        LifecyclePhaseTypeStatus lifecyclePhaseTypeStatus);
}
