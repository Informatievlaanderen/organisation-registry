namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Building;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class AddOrganisationBuildingInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public AddOrganisationBuildingRequest Body { get; }

        public AddOrganisationBuildingInternalRequest(Guid organisationId, AddOrganisationBuildingRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class AddOrganisationBuildingRequest
    {
        public Guid OrganisationBuildingId { get; set; }
        public Guid BuildingId { get; set; }
        public bool IsMainBuilding { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationBuildingInternalRequestValidator : AbstractValidator<AddOrganisationBuildingInternalRequest>
    {
        public AddOrganisationBuildingInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.BuildingId)
                .NotEmpty()
                .WithMessage("Building Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class AddOrganisationBuildingRequestMapping
    {
        public static AddOrganisationBuilding Map(AddOrganisationBuildingInternalRequest message)
        {
            return new AddOrganisationBuilding(
                message.Body.OrganisationBuildingId,
                new OrganisationId(message.OrganisationId),
                new BuildingId(message.Body.BuildingId),
                message.Body.IsMainBuilding,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
