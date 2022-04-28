namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRelationType;

    public class AddOrganisationRelationInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public AddOrganisationRelationRequest Body { get; }

        public AddOrganisationRelationInternalRequest(Guid organisationId, AddOrganisationRelationRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class AddOrganisationRelationRequest
    {
        public Guid OrganisationRelationId { get; set; }
        public Guid RelationId { get; set; }
        public Guid RelatedOrganisationId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationRelationInternalRequestValidator : AbstractValidator<AddOrganisationRelationInternalRequest>
    {
        public AddOrganisationRelationInternalRequestValidator()
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

    public static class AddOrganisationRelationRequestMapping
    {
        public static AddOrganisationRelation Map(AddOrganisationRelationInternalRequest message)
        {
            return new AddOrganisationRelation(
                message.Body.OrganisationRelationId,
                new OrganisationRelationTypeId(message.Body.RelationId),
                new OrganisationId(message.OrganisationId),
                new OrganisationId(message.Body.RelatedOrganisationId),
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
