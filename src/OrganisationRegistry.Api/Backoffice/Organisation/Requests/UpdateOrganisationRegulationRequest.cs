namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.RegulationTheme;
    using RegulationSubTheme;

    public class UpdateOrganisationRegulationInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationRegulationRequest Body { get; }

        public UpdateOrganisationRegulationInternalRequest(Guid organisationId, UpdateOrganisationRegulationRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationRegulationRequest
    {
        public Guid OrganisationRegulationId { get; set; }
        public Guid RegulationThemeId { get; set; }
        public Guid RegulationSubThemeId { get; set; }
        public DateTime? RegulationDate { get; set; }
        public string? RegulationUrl { get; set; }
        public string? Description { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationRegulationInternalRequestValidator : AbstractValidator<UpdateOrganisationRegulationInternalRequest>
    {
        public UpdateOrganisationRegulationInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class UpdateOrganisationRegulationRequestMapping
    {
        public static UpdateOrganisationRegulation Map(UpdateOrganisationRegulationInternalRequest message)
        {
            return new UpdateOrganisationRegulation(
                message.Body.OrganisationRegulationId,
                new OrganisationId(message.OrganisationId),
                new RegulationThemeId(message.Body.RegulationThemeId),
                new RegulationSubThemeId(message.Body.RegulationSubThemeId),
                message.Body.RegulationUrl,
                message.Body.RegulationDate,
                message.Body.Description,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
