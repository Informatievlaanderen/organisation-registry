namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Purpose;
    using SqlServer.Organisation;

    public class UpdateOrganisationInfoNotLimitedByVlimpersInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationInfoNotLimitedByVlimpersRequest Body { get; set; }

        public UpdateOrganisationInfoNotLimitedByVlimpersInternalRequest(Guid organisationId, UpdateOrganisationInfoNotLimitedByVlimpersRequest body)
        {
            OrganisationId = organisationId;
            Body = body;
        }
    }

    public class UpdateOrganisationInfoNotLimitedByVlimpersRequest
    {
        public string Description { get; set; }
        public List<Guid> PurposeIds { get; set; }
        public bool ShowOnVlaamseOverheidSites { get; set; }
    }

    public class UpdateOrganisationInfoNotLimitedByVlimpersRequestValidator : AbstractValidator<UpdateOrganisationInfoNotLimitedByVlimpersInternalRequest>
    {
        public UpdateOrganisationInfoNotLimitedByVlimpersRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");
        }
    }

    public static class UpdateOrganisationInfoNotLimitedByVlimpersRequestMapping
    {
        public static UpdateOrganisationInfoNotLimitedByVlimpers Map(UpdateOrganisationInfoNotLimitedByVlimpersInternalRequest message)
        {
            return new UpdateOrganisationInfoNotLimitedByVlimpers(
                new OrganisationId(message.OrganisationId),
                message.Body.Description,
                message.Body.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.Body.ShowOnVlaamseOverheidSites);
        }
    }
}
