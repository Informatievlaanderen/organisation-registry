namespace OrganisationRegistry.Api.LifecyclePhaseType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.LifecyclePhaseType;
    using OrganisationRegistry.LifecyclePhaseType;
    using OrganisationRegistry.LifecyclePhaseType.Commands;

    public class CreateLifecyclePhaseTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool RepresentsActivePhase { get; set; }

        public bool IsDefaultPhase { get; set; }
    }

    public class CreateLifecyclePhaseTypeRequestValidator : AbstractValidator<CreateLifecyclePhaseTypeRequest>
    {
        public CreateLifecyclePhaseTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, LifecyclePhaseTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LifecyclePhaseTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateLifecyclePhaseTypeRequestMapping
    {
        public static CreateLifecyclePhaseType Map(CreateLifecyclePhaseTypeRequest message)
        {
            return new CreateLifecyclePhaseType(
                new LifecyclePhaseTypeId(message.Id),
                new LifecyclePhaseTypeName(message.Name),
                message.RepresentsActivePhase ? LifecyclePhaseTypeIsRepresentativeFor.ActivePhase : LifecyclePhaseTypeIsRepresentativeFor.InactivePhase,
                message.IsDefaultPhase ? LifecyclePhaseTypeStatus.Default : LifecyclePhaseTypeStatus.NonDefault);
        }
    }
}
