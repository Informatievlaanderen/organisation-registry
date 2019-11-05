namespace OrganisationRegistry.Api.Configuration.Requests
{
    using FluentValidation;

    public class CreateConfigurationValueRequest
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
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
}
