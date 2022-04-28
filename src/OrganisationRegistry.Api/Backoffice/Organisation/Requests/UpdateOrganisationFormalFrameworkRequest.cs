namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using FormalFramework;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class UpdateOrganisationFormalFrameworkInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationFormalFrameworkRequest Body { get; }

        public UpdateOrganisationFormalFrameworkInternalRequest(Guid organisationId, UpdateOrganisationFormalFrameworkRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationFormalFrameworkRequest
    {
        public Guid OrganisationFormalFrameworkId { get; set; }
        public Guid FormalFrameworkId { get; set; }
        public Guid ParentOrganisationId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationFormalFrameworkInternalRequestValidator : AbstractValidator<UpdateOrganisationFormalFrameworkInternalRequest>
    {
        public UpdateOrganisationFormalFrameworkInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.FormalFrameworkId)
                .NotEmpty()
                .WithMessage("FormalFramework Organisation Id is required.");

            RuleFor(x => x.Body.ParentOrganisationId)
                .NotEmpty()
                .WithMessage("Parent Organisation Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");

            // TODO: Validate if org id is valid
        }
    }

    public static class UpdateOrganisationFormalFrameworkRequestMapping
    {
        public static UpdateOrganisationFormalFramework Map(UpdateOrganisationFormalFrameworkInternalRequest message)
        {
            return new UpdateOrganisationFormalFramework(
                message.Body.OrganisationFormalFrameworkId,
                new FormalFrameworkId(message.Body.FormalFrameworkId),
                new OrganisationId(message.OrganisationId),
                new OrganisationId(message.Body.ParentOrganisationId),
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
