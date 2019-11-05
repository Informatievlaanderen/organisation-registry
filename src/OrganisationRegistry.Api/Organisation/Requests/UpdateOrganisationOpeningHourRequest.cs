namespace OrganisationRegistry.Api.Organisation.Requests
{
    using FluentValidation;
    using System;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    public class UpdateOrganisationOpeningHourInternalRequest
    {
        public Guid OrganisationId { get; set; }

        public UpdateOrganisationOpeningHourRequest Body { get; }

        public UpdateOrganisationOpeningHourInternalRequest(
            Guid organisationId,
            UpdateOrganisationOpeningHourRequest message)
        {
            OrganisationId = organisationId;
            Body = message;
        }
    }

    public class UpdateOrganisationOpeningHourRequest
    {
        public Guid OrganisationOpeningHourId { get; set; }

        public TimeSpan Opens { get; set; }

        public TimeSpan Closes { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class UpdateOrganisationOpeningHourInternalRequestValidator : AbstractValidator<UpdateOrganisationOpeningHourInternalRequest>
    {
        public UpdateOrganisationOpeningHourInternalRequestValidator()
        {
            RuleFor(x => x.OrganisationId)
                .NotEmpty()
                .WithMessage("Organisation Id is required.");

            RuleFor(x => x.Body.Opens)
                .NotNull()
                .WithMessage("Opens is required.");

            RuleFor(x => x.Body.Closes)
                .NotNull()
                .WithMessage("Closes is required.");

            RuleFor(x => x.Body.ValidTo)
                .GreaterThanOrEqualTo(x => x.Body.ValidFrom)
                .When(x => x.Body.ValidFrom.HasValue)
                .WithMessage("Valid To must be greater than or equal to Valid From.");
        }
    }

    public static class UpdateOrganisationOpeningHourRequestMapping
    {
        public static UpdateOrganisationOpeningHour Map(UpdateOrganisationOpeningHourInternalRequest message)
        {
            return new UpdateOrganisationOpeningHour(
                message.Body.OrganisationOpeningHourId,
                new OrganisationId(message.OrganisationId),
                message.Body.Opens,
                message.Body.Closes,
                message.Body.DayOfWeek,
                new ValidFrom(message.Body.ValidFrom),
                new ValidTo(message.Body.ValidTo));
        }
    }
}
