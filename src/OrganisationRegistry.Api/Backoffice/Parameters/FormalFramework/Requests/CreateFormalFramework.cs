namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.FormalFramework;
    using OrganisationRegistry.FormalFramework.Commands;
    using OrganisationRegistry.FormalFrameworkCategory;
    using SqlServer.FormalFramework;

    public class CreateFormalFrameworkRequest
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public Guid FormalFrameworkCategoryId { get; set; }
    }

    public class CreateFormalFrameworkRequestValidator : AbstractValidator<CreateFormalFrameworkRequest>
    {
        public CreateFormalFrameworkRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Name)
                .Length(0, FormalFrameworkListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {FormalFrameworkListConfiguration.NameLength}.");

            RuleFor(x => x.Code)
                .NotEmpty()
                .WithMessage("Code is required.");

            RuleFor(x => x.Code)
                .Length(0, FormalFrameworkListConfiguration.CodeLength)
                .WithMessage($"Code cannot be longer than {FormalFrameworkListConfiguration.CodeLength}.");

            RuleFor(x => x.FormalFrameworkCategoryId)
                .NotEmpty()
                .WithMessage("FormalFrameworkCategoryId is required.");
        }
    }

    public static class CreateFormalFrameworkRequestMapping
    {
        public static CreateFormalFramework Map(CreateFormalFrameworkRequest message)
        {
            return new CreateFormalFramework(
                new FormalFrameworkId(message.Id),
                message.Name,
                message.Code,
                new FormalFrameworkCategoryId(message.FormalFrameworkCategoryId));
        }
    }
}
