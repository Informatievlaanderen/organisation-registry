namespace OrganisationRegistry.Organisation;

using System;

public class OrganisationOpeningHour : IOrganisationField, IValidityBuilder<OrganisationOpeningHour>
{
    public Guid Id => OrganisationOpeningHourId;
    public Guid OrganisationId { get; }
    public Guid OrganisationOpeningHourId { get; }
    public TimeSpan Opens { get; }
    public TimeSpan Closes { get; }
    public DayOfWeek? DayOfWeek { get; }
    public Period Validity { get; }

    public OrganisationOpeningHour(
        Guid organisationOpeningHourId,
        Guid organisationId,
        TimeSpan opens,
        TimeSpan closes,
        DayOfWeek? dayOfWeek,
        Period validity)
    {
        OrganisationOpeningHourId = organisationOpeningHourId;
        OrganisationId = organisationId;
        Opens = opens;
        Closes = closes;
        DayOfWeek = dayOfWeek;
        Validity = validity;
    }

    public OrganisationOpeningHour WithValidity(Period period)
        => new(
            OrganisationOpeningHourId,
            OrganisationId,
            Opens,
            Closes,
            DayOfWeek,
            period);

    public OrganisationOpeningHour WithValidFrom(ValidFrom validFrom)
        => WithValidity(new Period(validFrom, Validity.End));

    public OrganisationOpeningHour WithValidTo(ValidTo validTo)
        => WithValidity(new Period(Validity.Start, validTo));
}
