namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.ContactType;
using OrganisationRegistry.ContactType.Commands;
using SqlServer.ContactType;

public class CreateContactTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
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
        => new(
            new ContactTypeId(message.Id),
            message.Name);
}
