namespace OrganisationRegistry.Organisation.Commands
{
    using System;

    public class AddOrganisationOpeningHour : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationOpeningHourId { get; }
        public TimeSpan Opens { get; }
        public TimeSpan Closes { get; }
        public DayOfWeek? DayOfWeek { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public AddOrganisationOpeningHour(
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
}
