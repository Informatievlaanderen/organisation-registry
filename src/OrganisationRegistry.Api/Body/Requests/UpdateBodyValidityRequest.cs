namespace OrganisationRegistry.Api.Body.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;

    public class UpdateBodyValidityInternalRequest
    {
        public Guid BodyId { get; set; }
        public UpdateBodyValidityRequest Body { get; set; }

        public UpdateBodyValidityInternalRequest(Guid bodyId, UpdateBodyValidityRequest body)
        {
            BodyId = bodyId;
            Body = body;
        }
    }

    public class UpdateBodyValidityRequest
    {
        public DateTime? FormalValidFrom { get; set; }

        public DateTime? FormalValidTo { get; set; }
    }

    public class UpdateBodyValidityRequestValidator : AbstractValidator<UpdateBodyValidityInternalRequest>
    {
        public UpdateBodyValidityRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.FormalValidTo)
                .GreaterThanOrEqualTo(x => x.Body.FormalValidFrom)
                .When(x => x.Body.FormalValidFrom.HasValue)
                .WithMessage("Formal Valid To must be greater than or equal to Formal Valid From.");
        }
    }

    public static class UpdateBodyValidityRequestMapping
    {
        public static UpdateBodyValidity Map(UpdateBodyValidityInternalRequest message)
        {
            return new UpdateBodyValidity(
                new BodyId(message.BodyId),
                new Period(
                    new ValidFrom(message.Body.FormalValidFrom),
                    new ValidTo(message.Body.FormalValidTo)));
        }
    }
}
