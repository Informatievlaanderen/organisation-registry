namespace OrganisationRegistry.Api.FormalFrameworkCategory.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.FormalFrameworkCategory;
    using OrganisationRegistry.FormalFrameworkCategory;
    using OrganisationRegistry.FormalFrameworkCategory.Commands;

    public class UpdateFormalFrameworkCategoryInternalRequest
    {
        public Guid FormalFrameworkCategoryId { get; set; }
        public UpdateFormalFrameworkCategoryRequest Body { get; set; }

        public UpdateFormalFrameworkCategoryInternalRequest(Guid formalFrameworkCategoryId, UpdateFormalFrameworkCategoryRequest body)
        {
            FormalFrameworkCategoryId = formalFrameworkCategoryId;
            Body = body;
        }
    }

    public class UpdateFormalFrameworkCategoryRequest
    {
        public string Name { get; set; }
    }

    public class UpdateFormalFrameworkCategoryRequestValidator : AbstractValidator<UpdateFormalFrameworkCategoryInternalRequest>
    {
        public UpdateFormalFrameworkCategoryRequestValidator()
        {
            RuleFor(x => x.FormalFrameworkCategoryId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, FormalFrameworkCategoryListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {FormalFrameworkCategoryListConfiguration.NameLength}.");
        }
    }

    public static class UpdateFormalFrameworkCategoryRequestMapping
    {
        public static UpdateFormalFrameworkCategory Map(UpdateFormalFrameworkCategoryInternalRequest message)
        {
            return new UpdateFormalFrameworkCategory(
                new FormalFrameworkCategoryId(message.FormalFrameworkCategoryId),
                message.Body.Name);
        }
    }
}
