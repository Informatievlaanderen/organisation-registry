namespace OrganisationRegistry.Api.Backoffice.Body.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.FormalFramework;

    public class AddBodyFormalFrameworkInternalRequest
    {
        public Guid BodyId { get; set; }
        public AddBodyFormalFrameworkRequest Body { get; }

        public AddBodyFormalFrameworkInternalRequest(Guid bodyId, AddBodyFormalFrameworkRequest message)
        {
            BodyId = bodyId;
            Body = message;
        }
    }

    public class AddBodyFormalFrameworkRequest
    {
        public Guid BodyFormalFrameworkId { get; set; }
        public Guid FormalFrameworkId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddBodyFormalFrameworkInternalRequestValidator : AbstractValidator<AddBodyFormalFrameworkInternalRequest>
    {
        public AddBodyFormalFrameworkInternalRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BodyFormalFrameworkId)
                .NotEmpty()
                .WithMessage("Body Formal Framework Id is required.");

            RuleFor(x => x.Body.FormalFrameworkId)
                .NotEmpty()
                .WithMessage("Formal Framework Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class AddBodyFormalFrameworkRequestMapping
    {
        public static AddBodyFormalFramework Map(AddBodyFormalFrameworkInternalRequest message)
        {
            return new AddBodyFormalFramework(
                new BodyId(message.BodyId),
                new BodyFormalFrameworkId(message.Body.BodyFormalFrameworkId),
                new FormalFrameworkId(message.Body.FormalFrameworkId),
                new Period(
                    new ValidFrom(message.Body.ValidFrom),
                    new ValidTo(message.Body.ValidTo)));
        }
    }
}
