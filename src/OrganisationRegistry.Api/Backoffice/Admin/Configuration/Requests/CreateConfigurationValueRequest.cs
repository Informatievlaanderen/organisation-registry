namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration.Requests;

using FluentValidation;

public class CreateConfigurationValueRequest
{
    public string Key { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Value { get; set; } = null!;
}

public class CreateConfigurationValueRequestValidator : AbstractValidator<CreateConfigurationValueRequest>
{
    public CreateConfigurationValueRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key is required.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage("Value is required.");
    }
}
