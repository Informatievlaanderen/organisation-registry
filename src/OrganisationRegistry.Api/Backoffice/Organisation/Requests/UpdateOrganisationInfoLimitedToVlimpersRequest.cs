namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Organisation;

    public class UpdateOrganisationInfoLimitedToVlimpersInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public UpdateOrganisationInfoLimitedToVlimpersRequest Body { get; set; }

        public UpdateOrganisationInfoLimitedToVlimpersInternalRequest(Guid organisationId, UpdateOrganisationInfoLimitedToVlimpersRequest body)
        {
            OrganisationId = organisationId;
            Body = body;
        }
    }

    public class UpdateOrganisationInfoLimitedToVlimpersRequest
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public string? Article { get; set; }
        public DateTime? OperationalValidFrom { get; set; }
        public DateTime? OperationalValidTo { get; set; }
    }

    public class UpdateOrganisationInfoLimitedByVlimpersRequestValidator : AbstractValidator<UpdateOrganisationInfoLimitedToVlimpersInternalRequest>
    {
        public UpdateOrganisationInfoLimitedByVlimpersRequestValidator()
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

    public static class UpdateOrganisationInfoLimitedToVlimpersRequestMapping
    {
        public static UpdateOrganisationInfoLimitedToVlimpers Map(UpdateOrganisationInfoLimitedToVlimpersInternalRequest message)
        {
            return new UpdateOrganisationInfoLimitedToVlimpers(
                new OrganisationId(message.OrganisationId),
                message.Body.Name,
                Article.Parse(message.Body.Article),
                message.Body.ShortName,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo),
                new ValidFrom(message.Body.OperationalValidFrom),
                new ValidTo(message.Body.OperationalValidTo));
        }
    }
}
