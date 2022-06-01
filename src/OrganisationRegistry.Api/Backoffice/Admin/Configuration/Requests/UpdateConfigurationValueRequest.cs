namespace OrganisationRegistry.Api.Backoffice.Admin.Configuration.Requests;

using FluentValidation;

public class UpdateConfigurationValueInternalRequest
{
    public string Key { get; set; }
    public UpdateConfigurationValueRequest Body { get; set; }

    public UpdateConfigurationValueInternalRequest(string key, UpdateConfigurationValueRequest body)
    {
        Key = key;
        Body = body;
    }
}

public class UpdateConfigurationValueRequest
{
    public string Description { get; set; } = null!;
    public string Value { get; set; }= null!;
}

public class UpdateConfigurationValueRequestValidator : AbstractValidator<UpdateConfigurationValueInternalRequest>
{
    public UpdateConfigurationValueRequestValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .WithMessage("Key is required.");

        RuleFor(x => x.Body.Description)
            .NotEmpty()
            .WithMessage("Description is required.");

        RuleFor(x => x.Body.Value)
            .NotEmpty()
            .WithMessage("Value is required.");
    }
}
