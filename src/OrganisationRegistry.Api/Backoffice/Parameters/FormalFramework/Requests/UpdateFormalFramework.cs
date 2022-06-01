namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.FormalFramework;
using OrganisationRegistry.FormalFramework.Commands;
using OrganisationRegistry.FormalFrameworkCategory;
using SqlServer.FormalFramework;

public class UpdateFormalFrameworkInternalRequest
{
    public Guid FormalFrameworkId { get; set; }
    public UpdateFormalFrameworkRequest Body { get; set; }

    public UpdateFormalFrameworkInternalRequest(Guid formalFrameworkId, UpdateFormalFrameworkRequest body)
    {
        FormalFrameworkId = formalFrameworkId;
        Body = body;
    }
}

public class UpdateFormalFrameworkRequest
{
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    public Guid FormalFrameworkCategoryId { get; set; }
}

public class UpdateFormalFrameworkRequestValidator : AbstractValidator<UpdateFormalFrameworkInternalRequest>
{
    public UpdateFormalFrameworkRequestValidator()
    {
        RuleFor(x => x.FormalFrameworkId)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Body.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Body.Name)
            .Length(0, FormalFrameworkListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {FormalFrameworkListConfiguration.NameLength}.");

        RuleFor(x => x.Body.Code)
            .NotEmpty()
            .WithMessage("Code is required.");

        RuleFor(x => x.Body.Code)
            .Length(0, FormalFrameworkListConfiguration.CodeLength)
            .WithMessage($"Code cannot be longer than {FormalFrameworkListConfiguration.CodeLength}.");

        RuleFor(x => x.Body.FormalFrameworkCategoryId)
            .NotEmpty()
            .WithMessage("FormalFrameworkCategoryId is required.");
    }
}

public static class UpdateFormalFrameworkRequestMapping
{
    public static UpdateFormalFramework Map(UpdateFormalFrameworkInternalRequest message)
        => new(
            new FormalFrameworkId(message.FormalFrameworkId),
            message.Body.Name,
            message.Body.Code,
            new FormalFrameworkCategoryId(message.Body.FormalFrameworkCategoryId));
}
