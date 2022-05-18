namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase
{
    using System;
    using FluentValidation;
    using LifecyclePhaseType;
    using OrganisationRegistry.Body;

    public class AddBodyLifecyclePhaseInternalRequest
    {
        public Guid BodyId { get; set; }
        public AddBodyLifecyclePhaseRequest Body { get; }

        public AddBodyLifecyclePhaseInternalRequest(Guid bodyId, AddBodyLifecyclePhaseRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class AddBodyLifecyclePhaseRequest
    {
        public Guid BodyLifecyclePhaseId { get; set; }
        public Guid LifecyclePhaseTypeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddBodyLifecyclePhaseInternalRequestValidator : AbstractValidator<AddBodyLifecyclePhaseInternalRequest>
    {
        public AddBodyLifecyclePhaseInternalRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BodyLifecyclePhaseId)
                .NotEmpty()
                .WithMessage("Body Lifecycle Phase Id is required.");

            RuleFor(x => x.Body.LifecyclePhaseTypeId)
                .NotEmpty()
                .WithMessage("Lifecycle Phase Type Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class AddBodyLifecyclePhaseRequestMapping
    {
        public static AddBodyLifecyclePhase Map(AddBodyLifecyclePhaseInternalRequest message)
        {
            return new AddBodyLifecyclePhase(
                new BodyId(message.BodyId),
                new BodyLifecyclePhaseId(message.Body.BodyLifecyclePhaseId),
                new LifecyclePhaseTypeId(message.Body.LifecyclePhaseTypeId),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }
    }
}
