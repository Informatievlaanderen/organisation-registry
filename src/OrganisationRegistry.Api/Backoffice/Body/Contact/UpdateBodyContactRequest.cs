namespace OrganisationRegistry.Api.Backoffice.Body.Contact
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using ContactType;
    using OrganisationRegistry.SqlServer.Body;

    public class UpdateBodyContactInternalRequest
    {
        public Guid BodyId { get; set; }
        public UpdateBodyContactRequest Body { get; }

        public UpdateBodyContactInternalRequest(Guid organisationId, UpdateBodyContactRequest message)
        {
            BodyId = organisationId;
            Body = message;
        }
    }

    public class UpdateBodyContactRequest
    {
        public Guid BodyContactId { get; set; }
        public Guid ContactTypeId { get; set; }
        public string ContactValue { get; set; } = null!;
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateBodyContactInternalRequestValidator : AbstractValidator<UpdateBodyContactInternalRequest>
    {
        public UpdateBodyContactInternalRequestValidator()
        {
            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.ContactTypeId)
                .NotEmpty()
                .WithMessage("Contact Type Id is required.");

            RuleFor(x => x.Body.ContactValue)
                .NotEmpty()
                .WithMessage("Contact Value is required.");

            RuleFor(x => x.Body.ContactValue)
                .Length(0, BodyContactListConfiguration.ContactValueLength)
                .WithMessage($"Contact Value cannot be longer than {BodyContactListConfiguration.ContactValueLength}.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.BodyId)
                .NotEmpty()
                .WithMessage("Body Id is required.");
        }
    }

    public static class UpdateBodyContactRequestMapping
    {
        public static UpdateBodyContact Map(UpdateBodyContactInternalRequest message)
        {
            return new UpdateBodyContact(
                message.Body.BodyContactId,
                new BodyId(message.BodyId),
                new ContactTypeId(message.Body.ContactTypeId),
                message.Body.ContactValue,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
