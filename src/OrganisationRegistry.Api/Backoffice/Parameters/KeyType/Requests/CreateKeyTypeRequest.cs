namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Requests;

using System;
using FluentValidation;
using KeyTypes;
using KeyTypes.Commands;
using SqlServer.KeyType;

public class CreateKeyTypeRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreateKeyTypeRequestValidator : AbstractValidator<CreateKeyTypeRequest>
{
    public CreateKeyTypeRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, KeyTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {KeyTypeListConfiguration.NameLength}.");
    }
}

public static class CreateKeyTypeRequestMapping
{
    public static CreateKeyType Map(CreateKeyTypeRequest message)
        => new(
            new KeyTypeId(message.Id),
            new KeyTypeName(message.Name));
}