namespace OrganisationRegistry.Day
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class DayId : GuidValueObject<DayId>
    {
        public DayId([JsonProperty("id")] Guid dayId) : base(dayId) { }
    }
}
