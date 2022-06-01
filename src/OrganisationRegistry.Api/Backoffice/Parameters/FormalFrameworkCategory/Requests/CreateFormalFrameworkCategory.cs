namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFrameworkCategory.Requests;

using System;
using FluentValidation;
using OrganisationRegistry.FormalFrameworkCategory;
using OrganisationRegistry.FormalFrameworkCategory.Commands;
using SqlServer.FormalFrameworkCategory;

public class CreateFormalFrameworkCategoryRequest
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class CreateFormalFrameworkCategoryRequestValidator : AbstractValidator<CreateFormalFrameworkCategoryRequest>
{
    public CreateFormalFrameworkCategoryRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is required.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is required.");

        RuleFor(x => x.Name)
            .Length(0, FormalFrameworkCategoryListConfiguration.NameLength)
            .WithMessage($"Name cannot be longer than {FormalFrameworkCategoryListConfiguration.NameLength}.");
    }
}

public static class CreateFormalFrameworkCategoryRequestMapping
{
    public static CreateFormalFrameworkCategory Map(CreateFormalFrameworkCategoryRequest message)
        => new(
            new FormalFrameworkCategoryId(message.Id),
            message.Name);
}
