namespace OrganisationRegistry.Api.ContactType.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.ContactType.Commands;
    using SqlServer.ContactType;
    using OrganisationRegistry.ContactType;

    public class UpdateContactTypeInternalRequest
    {
        public Guid ContactTypeId { get; set; }
        public UpdateContactTypeRequest Body { get; set; }

        public UpdateContactTypeInternalRequest(Guid contactTypeId, UpdateContactTypeRequest body)
        {
            ContactTypeId = contactTypeId;
            Body = body;
        }
    }

    public class UpdateContactTypeRequest
    {
        public string Name { get; set; }
    }

    public class UpdateContactTypeRequestValidator : AbstractValidator<UpdateContactTypeInternalRequest>
    {
        public UpdateContactTypeRequestValidator()
        {
            RuleFor(x => x.ContactTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, ContactTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {ContactTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateContactTypeRequestMapping
    {
        public static UpdateContactType Map(UpdateContactTypeInternalRequest message)
        {
            return new UpdateContactType(
                new ContactTypeId(message.ContactTypeId),
                message.Body.Name);
        }
    }
}
