namespace OrganisationRegistry.Api.ContactType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.ContactType;
    using OrganisationRegistry.ContactType;
    using OrganisationRegistry.ContactType.Commands;

    public class CreateContactTypeRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
    }

    public class CreateContactTypeRequestValidator : AbstractValidator<CreateContactTypeRequest>
    {
        public CreateContactTypeRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, ContactTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {ContactTypeListConfiguration.NameLength}.");
        }
    }

    public static class CreateContactTypeRequestMapping
    {
        public static CreateContactType Map(CreateContactTypeRequest message)
        {
            return new CreateContactType(
                new ContactTypeId(message.Id),
                message.Name);
        }
    }
}
