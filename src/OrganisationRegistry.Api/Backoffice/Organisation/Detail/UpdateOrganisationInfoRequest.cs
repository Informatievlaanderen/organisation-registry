namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Purpose;
    using OrganisationRegistry.SqlServer.Organisation;

    public class UpdateOrganisationInfoInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationInfoRequest Body { get; set; }

        public UpdateOrganisationInfoInternalRequest(Guid organisationId, UpdateOrganisationInfoRequest body)
        {
            OrganisationId = organisationId;
            Body = body;
        }
    }

    public class UpdateOrganisationInfoRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortName { get; set; }
        public List<Guid> PurposeIds { get; set; }
        public bool ShowOnVlaamseOverheidSites { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Article { get; set; }
        public DateTime? OperationalValidFrom { get; set; }
        public DateTime? OperationalValidTo { get; set; }
    }

    public class UpdateOrganisationInfoRequestValidator : AbstractValidator<UpdateOrganisationInfoInternalRequest>
    {
        public UpdateOrganisationInfoRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, OrganisationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationListConfiguration.NameLength}.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class UpdateOrganisationInfoRequestMapping
    {
        public static UpdateOrganisationInfo Map(UpdateOrganisationInfoInternalRequest message)
        {
            return new UpdateOrganisationInfo(
                new OrganisationId(message.OrganisationId),
                message.Body.Name,
                Article.Parse(message.Body.Article),
                message.Body.Description,
                message.Body.ShortName,
                message.Body.PurposeIds?.Select(x => new PurposeId(x)).ToList(),
                message.Body.ShowOnVlaamseOverheidSites,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo),
                new ValidFrom(message.Body.OperationalValidFrom),
                new ValidTo(message.Body.OperationalValidTo));
        }
    }
}
