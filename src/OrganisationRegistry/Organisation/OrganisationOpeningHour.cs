namespace OrganisationRegistry.Organisation
{
    using System;

    public class OrganisationOpeningHour
    {
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
    }
}
