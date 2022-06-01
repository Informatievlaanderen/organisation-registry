namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Requests;

using System;
using FluentValidation;
using KeyTypes;
using KeyTypes.Commands;
using SqlServer.KeyType;

public class UpdateKeyTypeInternalRequest
{
    public Guid KeyTypeId { get; set; }
    public UpdateKeyTypeRequest Body { get; set; }

    public UpdateKeyTypeInternalRequest(Guid keyTypeId, UpdateKeyTypeRequest body)
    {
        KeyTypeId = keyTypeId;
        Body = body;
    }
}

public class UpdateKeyTypeRequest
{
    public string Name { get; set; } = null!;
}

public class UpdateKeyTypeRequestValidator : AbstractValidator<UpdateKeyTypeInternalRequest>
{
    public UpdateKeyTypeRequestValidator()
    {
        RuleFor(x => x.KeyTypeId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, KeyTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {KeyTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateKeyTypeRequestMapping
{
    public static UpdateKeyType Map(UpdateKeyTypeInternalRequest message)
        => new(
            new KeyTypeId(message.KeyTypeId),
            new KeyTypeName(message.Body.Name));
}
