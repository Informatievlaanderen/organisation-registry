namespace OrganisationRegistry.Organisation;

using System;

public class UpdateOrganisationOpeningHour : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationOpeningHourId { get; }
    public TimeSpan Opens { get; }
    public TimeSpan Closes { get; }
    public DayOfWeek? DayOfWeek { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public UpdateOrganisationOpeningHour(
        Guid organisationOpeningHourId,
        OrganisationId organisationId,
        TimeSpan opens,
        TimeSpan closes,
        DayOfWeek? dayOfWeek,
        ValidFrom validFrom,
        ValidTo validTo)
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
