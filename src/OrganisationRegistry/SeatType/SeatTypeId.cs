namespace OrganisationRegistry.SeatType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class SeatTypeId : GuidValueObject<SeatTypeId>
    {
        public SeatTypeId([JsonProperty("id")] Guid seatTypeId) : base(seatTypeId) { }
    }
}
