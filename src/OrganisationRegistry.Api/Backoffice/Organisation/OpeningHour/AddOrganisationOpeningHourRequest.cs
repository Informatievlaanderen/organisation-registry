namespace OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour;

using System;
using FluentValidation;
using OrganisationRegistry.Organisation;

public class AddOrganisationOpeningHourInternalRequest
{
    public Guid OrganisationId { get; set; }

    public AddOrganisationOpeningHourRequest Body { get; }

    public AddOrganisationOpeningHourInternalRequest(
        Guid organisationId,
        AddOrganisationOpeningHourRequest message)
    {
        OrganisationId = organisationId;
        Body = message;
    }
}

public class AddOrganisationOpeningHourRequest
{
    public Guid OrganisationOpeningHourId { get; set; }

    public TimeSpan Opens { get; set; }

    public TimeSpan Closes { get; set; }

    public DayOfWeek? DayOfWeek { get; set; }

    public DateTime? ValidFrom { get; set; }

    public DateTime? ValidTo { get; set; }
}

public class AddOrganisationOpeningHourInternalRequestValidator : AbstractValidator<AddOrganisationOpeningHourInternalRequest>
{
    public AddOrganisationOpeningHourInternalRequestValidator()
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

public static class AddOrganisationOpeningHourRequestMapping
{
    public static AddOrganisationOpeningHour Map(AddOrganisationOpeningHourInternalRequest message)
    {
        return new AddOrganisationOpeningHour(
            message.Body.OrganisationOpeningHourId,
            new OrganisationId(message.OrganisationId),
            message.Body.Opens,
            message.Body.Closes,
            message.Body.DayOfWeek,
            new ValidFrom(message.Body.ValidFrom),
            new ValidTo(message.Body.ValidTo));
    }
}
