namespace OrganisationRegistry.Api.Organisation.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.Organisation;
    using OrganisationRegistry.RegulationType;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

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
        public Guid RegulationTypeId { get; set; }
        public DateTime? Date { get; set; }
        public string? Link { get; set; }
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
                new RegulationTypeId(message.Body.RegulationTypeId),
                message.Body.Link,
                message.Body.Date,
                message.Body.Description,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
