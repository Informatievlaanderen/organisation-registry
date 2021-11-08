﻿namespace OrganisationRegistry.Api.Backoffice.Organisation.Requests
{
    using System;
    using FluentValidation;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.RegulationTheme;

    public class AddOrganisationRegulationInternalRequest
    {
        public Guid OrganisationId { get; set; }
        public AddOrganisationRegulationRequest Body { get; }

        public AddOrganisationRegulationInternalRequest(Guid organisationId, AddOrganisationRegulationRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class AddOrganisationRegulationRequest
    {
        public Guid OrganisationRegulationId { get; set; }
        public Guid RegulationThemeId { get; set; }
        public DateTime? Date { get; set; }
        public string? Link { get; set; }
        public string? Description { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
    }

    public class AddOrganisationRegulationInternalRequestValidator : AbstractValidator<AddOrganisationRegulationInternalRequest>
    {
        public AddOrganisationRegulationInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");

            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");
        }
    }

    public static class AddOrganisationRegulationRequestMapping
    {
        public static AddOrganisationRegulation Map(AddOrganisationRegulationInternalRequest message)
        {
            return new AddOrganisationRegulation(
                message.Body.OrganisationRegulationId,
                new OrganisationId(message.OrganisationId),
                new RegulationThemeId(message.Body.RegulationThemeId),
                message.Body.Link,
                message.Body.Date,
                message.Body.Description,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
