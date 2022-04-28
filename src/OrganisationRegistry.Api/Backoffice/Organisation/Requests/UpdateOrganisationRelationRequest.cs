namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRelationType;

    public class UpdateOrganisationRelationInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationRelationRequest Body { get; }

        public UpdateOrganisationRelationInternalRequest(Guid organisationId, UpdateOrganisationRelationRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationRelationRequest
    {
        public Guid OrganisationRelationId { get; set; }
        public Guid RelationId { get; set; }
        public Guid RelatedOrganisationId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationRelationInternalRequestValidator : AbstractValidator<UpdateOrganisationRelationInternalRequest>
    {
        public UpdateOrganisationRelationInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.RelationId)
                .NotEmpty()
                .WithMessage("Relation Id is required.");

            RuleFor(x => x.Body.RelatedOrganisationId)
                .NotEmpty()
                .WithMessage("Related Organisation Id is required.");

            // TODO: Validate if RelationTypeId is valid

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

    public static class UpdateOrganisationRelationRequestMapping
    {
        public static UpdateOrganisationRelation Map(UpdateOrganisationRelationInternalRequest message)
        {
            return new UpdateOrganisationRelation(
                message.Body.OrganisationRelationId,
                new OrganisationRelationTypeId(message.Body.RelationId),
                new OrganisationId(message.OrganisationId),
                new OrganisationId(message.Body.RelatedOrganisationId),
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
