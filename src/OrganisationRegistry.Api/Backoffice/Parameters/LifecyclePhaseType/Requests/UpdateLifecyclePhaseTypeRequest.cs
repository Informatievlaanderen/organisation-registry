namespace OrganisationRegistry.Api.Backoffice.Parameters.LifecyclePhaseType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.LifecyclePhaseType;
    using OrganisationRegistry.LifecyclePhaseType.Commands;
    using SqlServer.LifecyclePhaseType;

    public class UpdateLifecyclePhaseTypeInternalRequest
    {
        public Guid LifecyclePhaseTypeId { get; set; }
        public UpdateLifecyclePhaseTypeRequest Body { get; set; }

        public UpdateLifecyclePhaseTypeInternalRequest(Guid lifecyclePhaseTypeId, UpdateLifecyclePhaseTypeRequest body)
        {
            LifecyclePhaseTypeId = lifecyclePhaseTypeId;
            Body = body;
        }
    }

    public class UpdateLifecyclePhaseTypeRequest
    {
        public string Name { get; set; } = null!;

        public bool RepresentsActivePhase { get; set; }

        public bool IsDefaultPhase { get; set; }
    }

    public class UpdateLifecyclePhaseTypeRequestValidator : AbstractValidator<UpdateLifecyclePhaseTypeInternalRequest>
    {
        public UpdateLifecyclePhaseTypeRequestValidator()
        {
            RuleFor(x => x.LifecyclePhaseTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, LifecyclePhaseTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {LifecyclePhaseTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateLifecyclePhaseTypeRequestMapping
    {
        public static UpdateLifecyclePhaseType Map(UpdateLifecyclePhaseTypeInternalRequest message)
            => new(
                new LifecyclePhaseTypeId(message.LifecyclePhaseTypeId),
                new LifecyclePhaseTypeName(message.Body.Name),
                message.Body.RepresentsActivePhase ? LifecyclePhaseTypeIsRepresentativeFor.ActivePhase : LifecyclePhaseTypeIsRepresentativeFor.InactivePhase,
                message.Body.IsDefaultPhase ? LifecyclePhaseTypeStatus.Default : LifecyclePhaseTypeStatus.NonDefault);
    }
}
