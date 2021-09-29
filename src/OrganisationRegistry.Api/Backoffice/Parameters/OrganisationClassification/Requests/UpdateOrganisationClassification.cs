namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.OrganisationClassification;
    using OrganisationRegistry.OrganisationClassification.Commands;
    using OrganisationRegistry.OrganisationClassificationType;
    using SqlServer.OrganisationClassification;

    public class UpdateOrganisationClassificationInternalRequest
    {
        public Guid OrganisationClassificationId { get; set; }
        public UpdateOrganisationClassificationRequest Body { get; set; }

        public UpdateOrganisationClassificationInternalRequest(Guid organisationClassificationId, UpdateOrganisationClassificationRequest body)
        {
            OrganisationClassificationId = organisationClassificationId;
            Body = body;
        }
    }

    public class UpdateOrganisationClassificationRequest
    {
        public string Name { get; set; }
        public int Order { get; set; }
        public string ExternalKey { get; set; }
        public bool Active { get; set; }
        public Guid OrganisationClassificationTypeId { get; set; }
    }

    public class UpdateOrganisationClassificationRequestValidator : AbstractValidator<UpdateOrganisationClassificationInternalRequest>
    {
        public UpdateOrganisationClassificationRequestValidator()
        {
            RuleFor(x => x.OrganisationClassificationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, OrganisationClassificationListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationClassificationListConfiguration.NameLength}.");

            RuleFor(x => x.Body.Order)
                .GreaterThan(0)
                .WithMessage("Order must be greater than 0.");

            RuleFor(x => x.Body.OrganisationClassificationTypeId)
                .NotEmpty()
                .WithMessage("OrganisationClassificationTypeId is required.");
        }
    }

    public static class UpdateOrganisationClassificationRequestMapping
    {
        public static UpdateOrganisationClassification Map(UpdateOrganisationClassificationInternalRequest message)
            => new UpdateOrganisationClassification(
                new OrganisationClassificationId(message.OrganisationClassificationId),
                new OrganisationClassificationName(message.Body.Name),
                message.Body.Order,
                message.Body.ExternalKey,
                message.Body.Active,
                new OrganisationClassificationTypeId(message.Body.OrganisationClassificationTypeId));
    }
}
