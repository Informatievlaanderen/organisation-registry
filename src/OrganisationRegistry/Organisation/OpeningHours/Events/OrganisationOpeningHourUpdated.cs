namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationOpeningHourUpdated : BaseEvent<OrganisationOpeningHourUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationOpeningHourId { get; }

    public TimeSpan Opens { get; }
    public TimeSpan PreviousOpens { get; }

    public TimeSpan Closes { get; }
    public TimeSpan PreviousCloses { get; }

    public DayOfWeek? DayOfWeek { get; set; }
    public DayOfWeek? PreviousDayOfWeek { get; set; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviousValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviousValidTo { get; }

    public OrganisationOpeningHourUpdated(
        Guid organisationId,
        Guid organisationOpeningHourId,
        TimeSpan opens,
        TimeSpan previousOpens,
        TimeSpan closes,
        TimeSpan previousCloses,
        DayOfWeek? dayOfWeek,
        DayOfWeek? previousDayOfWeek,
        DateTime? validFrom,
        DateTime? previousValidFrom,
        DateTime? validTo,
        DateTime? previousValidTo)
    {
        Id = organisationId;

        OrganisationOpeningHourId = organisationOpeningHourId;
        Opens = opens;
        Closes = closes;
        DayOfWeek = dayOfWeek;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousOpens = previousOpens;
        PreviousCloses = previousCloses;
        PreviousDayOfWeek = previousDayOfWeek;
        PreviousValidFrom = previousValidFrom;
        PreviousValidTo = previousValidTo;
    }
}