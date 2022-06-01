namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.Purpose;
using OrganisationRegistry.Purpose.Commands;
using SqlServer.Purpose;

public class CreatePurposeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreatePurposeRequestValidator : AbstractValidator<CreatePurposeRequest>
{
    public CreatePurposeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, PurposeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {PurposeListConfiguration.NameLength}.");
    }
}

public static class CreatePurposeRequestMapping
{
    public static CreatePurpose Map(CreatePurposeRequest message)
        => new(
            new PurposeId(message.Id),
            new PurposeName(message.Name));
}
