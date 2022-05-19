namespace OrganisationRegistry.Api.Backoffice.Organisation.Building
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Building;
    using OrganisationRegistry.Organisation;

    public class UpdateOrganisationBuildingInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationBuildingRequest Body { get; }

        public UpdateOrganisationBuildingInternalRequest(Guid organisationId, UpdateOrganisationBuildingRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationBuildingRequest
    {
        public Guid OrganisationBuildingId { get; set; }
        public Guid BuildingId { get; set; }
        public bool IsMainBuilding { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationBuildingInternalRequestValidator : AbstractValidator<UpdateOrganisationBuildingInternalRequest>
    {
        public UpdateOrganisationBuildingInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BuildingId)
                .NotEmpty()
                .WithMessage("Building Id is required.");

            // TODO: Validate if BuildingId is valid

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From");

            // TODO: Validate if org id is valid
        }
    }

    public static class UpdateOrganisationBuildingRequestMapping
    {
        public static UpdateOrganisationBuilding Map(UpdateOrganisationBuildingInternalRequest message)
        {
            return new UpdateOrganisationBuilding(
                message.Body.OrganisationBuildingId,
                new OrganisationId(message.OrganisationId),
                new BuildingId(message.Body.BuildingId),
                message.Body.IsMainBuilding,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
