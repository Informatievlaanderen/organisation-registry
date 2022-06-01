namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.LabelType;
using OrganisationRegistry.LabelType.Commands;
using SqlServer.LabelType;

public class UpdateLabelTypeInternalRequest
{
    public Guid LabelTypeId { get; set; }
    public UpdateLabelTypeRequest Body { get; set; }

    public UpdateLabelTypeInternalRequest(Guid labelTypeId, UpdateLabelTypeRequest body)
    {
        LabelTypeId = labelTypeId;
        Body = body;
    }
}

public class UpdateLabelTypeRequest
{
    public string Name { get; set; } = null!;
}

public class UpdateLabelTypeRequestValidator : AbstractValidator<UpdateLabelTypeInternalRequest>
{
    public UpdateLabelTypeRequestValidator()
    {
        RuleFor(x => x.LabelTypeId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, LabelTypeListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {LabelTypeListConfiguration.NameLength}.");
    }
}

public static class UpdateLabelTypeRequestMapping
{
    public static UpdateLabelType Map(UpdateLabelTypeInternalRequest message)
        => new(
            new LabelTypeId(message.LabelTypeId),
            new LabelTypeName(message.Body.Name));
}
