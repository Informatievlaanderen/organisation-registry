namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassificationType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.BodyClassificationType;
using OrganisationRegistry.BodyClassificationType.Commands;
using SqlServer.BodyClassificationType;

public class UpdateBodyClassificationTypeInternalRequest
{
    public Guid BodyClassificationTypeId { get; set; }
    public UpdateBodyClassificationTypeRequest Body { get; set; }

    public UpdateBodyClassificationTypeInternalRequest(Guid bodyClassificationTypeId, UpdateBodyClassificationTypeRequest body)
    {
        BodyClassificationTypeId = bodyClassificationTypeId;
        Body = body;
    }
}

public class UpdateBodyClassificationTypeRequest
{
    public string Name { get; set; } = null!;
}

public class UpdateBodyClassificationTypeRequestValidator : AbstractValidator<UpdateBodyClassificationTypeInternalRequest>
{
    public UpdateBodyClassificationTypeRequestValidator()
    {
        RuleFor(x => x.BodyClassificationTypeId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, BodyClassificationTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {BodyClassificationTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateBodyClassificationTypeRequestMapping
{
    public static UpdateBodyClassificationType Map(UpdateBodyClassificationTypeInternalRequest message)
        => new(
            new BodyClassificationTypeId(message.BodyClassificationTypeId),
            message.Body.Name);
}
