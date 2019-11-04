namespace OrganisationRegistry.Day.Events
{
    using System;

    public class DayHasPassed : BaseEvent<DayHasPassed>
    {
        public Guid DayId => Id; // this is silly but infra demands it. TODO: could we work around this?

        public DateTime Date { get; }

        public DateTime NextDate => Date.AddDays(1);

        public DayHasPassed(Guid dayId, DateTime date)
        {
            Id = dayId;

            Date = date;
        }
    }
}
