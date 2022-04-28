namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using ContactType;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Organisation;

    public class AddOrganisationContactInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public AddOrganisationContactRequest Body { get; }

        public AddOrganisationContactInternalRequest(Guid organisationId, AddOrganisationContactRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class AddOrganisationContactRequest
    {
        public Guid OrganisationContactId { get; set; }
        public Guid ContactTypeId { get; set; }
        public string ContactValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationContactInternalRequestValidator : AbstractValidator<AddOrganisationContactInternalRequest>
    {
        public AddOrganisationContactInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.ContactTypeId)
                .NotEmpty()
                .WithMessage("Contact Type Id is required.");

            RuleFor(x => x.Body.ContactValue)
                .NotEmpty()
                .WithMessage("Contact Value is required.");

            RuleFor(x => x.Body.ContactValue)
                .Length(0, OrganisationContactListConfiguration.ContactValueLength)
                .WithMessage($"Contact Value cannot be longer than {OrganisationContactListConfiguration.ContactValueLength}.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class AddOrganisationContactRequestMapping
    {
        public static AddOrganisationContact Map(AddOrganisationContactInternalRequest message)
        {
            return new AddOrganisationContact(
                message.Body.OrganisationContactId,
                new OrganisationId(message.OrganisationId),
                new ContactTypeId(message.Body.ContactTypeId),
                message.Body.ContactValue,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
