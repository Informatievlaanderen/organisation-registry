namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.UpdateNotLimitedToVlimpers;
    using Purpose;

    public class UpdateOrganisationInfoNotLimitedToVlimpersInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationInfoNotLimitedToVlimpersRequest Body { get; set; }

        public UpdateOrganisationInfoNotLimitedToVlimpersInternalRequest(Guid organisationId, UpdateOrganisationInfoNotLimitedToVlimpersRequest body)
        {
            OrganisationId = organisationId;
            Body = body;
        }
    }

    public class UpdateOrganisationInfoNotLimitedToVlimpersRequest
    {
        public string Description { get; set; }
        public List<Guid> PurposeIds { get; set; }
        public bool ShowOnVlaamseOverheidSites { get; set; }
    }

    public class UpdateOrganisationInfoNotLimitedByVlimpersRequestValidator : AbstractValidator<UpdateOrganisationInfoNotLimitedToVlimpersInternalRequest>
    {
        public UpdateOrganisationInfoNotLimitedByVlimpersRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");
        }
    }

    public static class UpdateOrganisationInfoNotLimitedToVlimpersRequestMapping
    {
        public static UpdateOrganisationInfoNotLimitedToVlimpers Map(UpdateOrganisationInfoNotLimitedToVlimpersInternalRequest message)
        {
            return new UpdateOrganisationInfoNotLimitedToVlimpers(
                new OrganisationId(message.OrganisationId),
                message.Body.Description,
                message.Body.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.Body.ShowOnVlaamseOverheidSites);
        }
    }
}
