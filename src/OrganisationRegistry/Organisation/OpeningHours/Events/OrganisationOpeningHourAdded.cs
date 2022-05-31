namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationOpeningHourAdded : BaseEvent<OrganisationOpeningHourAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationOpeningHourId { get; }
    public TimeSpan Opens { get; }
    public TimeSpan Closes { get; }
    public DayOfWeek? DayOfWeek { get; set; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationOpeningHourAdded(
        Guid organisationId,
        Guid organisationOpeningHourId,
        TimeSpan opens,
        TimeSpan closes,
        DayOfWeek? dayOfWeek,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = organisationId;

        OrganisationOpeningHourId = organisationOpeningHourId;
        Opens = opens;
        Closes = closes;
        DayOfWeek = dayOfWeek;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
