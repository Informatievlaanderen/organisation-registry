namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using Location;
    using LocationType;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class UpdateOrganisationLocationInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationLocationRequest Body { get; }

        public UpdateOrganisationLocationInternalRequest(Guid organisationId, UpdateOrganisationLocationRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationLocationRequest
    {
        public Guid OrganisationLocationId { get; set; }
        public Guid LocationId { get; set; }
        public bool IsMainLocation { get; set; }
        public Guid? LocationTypeId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Source { get; set; }
    }

    public class UpdateOrganisationLocationInternalRequestValidator : AbstractValidator<UpdateOrganisationLocationInternalRequest>
    {
        public UpdateOrganisationLocationInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.LocationId)
                .NotEmpty()
                .WithMessage("Location Id is required.");

            // TODO: Validate if LocationId is valid

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

    public static class UpdateOrganisationLocationRequestMapping
    {
        public static UpdateOrganisationLocation Map(UpdateOrganisationLocationInternalRequest message)
        {
            return new UpdateOrganisationLocation(
                message.Body.OrganisationLocationId,
                new OrganisationId(message.OrganisationId),
                new LocationId(message.Body.LocationId),
                message.Body.IsMainLocation,
                message.Body.LocationTypeId.HasValue ? new LocationTypeId(message.Body.LocationTypeId.Value) : null,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo),
                new Source(message.Body.Source));
        }
    }
}
