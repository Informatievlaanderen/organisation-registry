namespace OrganisationRegistry.Day
{
    using System;
    using Events;
    using Infrastructure.Domain;

    public class Day : AggregateRoot
    {
        private DateTime _previousDay;

        private Day() { }

        public Day(IDateTimeProvider dateTimeProvider, DayId dayId)
        {
            ApplyChange(new DayHasPassed(dayId, dateTimeProvider.Today.AddDays(-1)));
        }

        public void CheckIfDayHasPassed(IDateTimeProvider dateTimeProvider)
        {
            var yesterday = dateTimeProvider.Today.AddDays(-1);
            if (_previousDay.Date < yesterday)
                ApplyChange(new DayHasPassed(Id, yesterday));
        }

        private void Apply(DayHasPassed @event)
        {
            Id = @event.DayId;
            _previousDay = @event.Date;
        }
    }
}
