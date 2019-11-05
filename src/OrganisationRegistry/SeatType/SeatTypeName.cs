namespace OrganisationRegistry.SeatType
{
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class SeatTypeName : StringValueObject<SeatTypeName>
    {
        public SeatTypeName([JsonProperty("name")] string seatTypeName) : base(seatTypeName) { }
    }
}
