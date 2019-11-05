namespace OrganisationRegistry.Api.OrganisationClassificationType.Requests
{
    using System;
    using FluentValidation;
    using SqlServer.OrganisationClassificationType;
    using OrganisationRegistry.OrganisationClassificationType;
    using OrganisationRegistry.OrganisationClassificationType.Commands;

    public class UpdateOrganisationClassificationTypeInternalRequest
    {
        public Guid OrganisationClassificationTypeId { get; set; }
        public UpdateOrganisationClassificationTypeRequest Body { get; set; }

        public UpdateOrganisationClassificationTypeInternalRequest(Guid organisationClassificationTypeId, UpdateOrganisationClassificationTypeRequest body)
        {
            OrganisationClassificationTypeId = organisationClassificationTypeId;
            Body = body;
        }
    }

    public class UpdateOrganisationClassificationTypeRequest
    {
        public string Name { get; set; }
    }

    public class UpdateOrganisationClassificationTypeRequestValidator : AbstractValidator<UpdateOrganisationClassificationTypeInternalRequest>
    {
        public UpdateOrganisationClassificationTypeRequestValidator()
        {
            RuleFor(x => x.OrganisationClassificationTypeId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.Name)
                .NotEmpty()
                .WithMessage("Name is required.");

            RuleFor(x => x.Body.Name)
                .Length(0, OrganisationClassificationTypeListConfiguration.NameLength)
                .WithMessage($"Name cannot be longer than {OrganisationClassificationTypeListConfiguration.NameLength}.");
        }
    }

    public static class UpdateOrganisationClassificationTypeRequestMapping
    {
        public static UpdateOrganisationClassificationType Map(UpdateOrganisationClassificationTypeInternalRequest message)
        {
            return new UpdateOrganisationClassificationType(
                new OrganisationClassificationTypeId(message.OrganisationClassificationTypeId),
                message.Body.Name);
        }
    }
}
