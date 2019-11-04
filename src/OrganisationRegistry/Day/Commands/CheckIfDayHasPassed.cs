namespace OrganisationRegistry.Day.Commands
{
    using System;

    public class CheckIfDayHasPassed : BaseCommand<DayId>
    {
        private static readonly DayId ManMadeGuid = new DayId(Guid.Parse("00000000-0000-4000-0000-000000000001"));

        public DayId DayId => Id;

        public CheckIfDayHasPassed()
        {
            Id = ManMadeGuid;
        }
    }
}
