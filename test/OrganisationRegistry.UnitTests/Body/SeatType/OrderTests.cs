namespace OrganisationRegistry.UnitTests.Body.SeatType
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using OrganisationRegistry.SeatType.Events;
    using Xunit;

    public class OrderTests
    {
        [Fact]
        public void CanDeserializeWithAndWithoutOrderString()
        {
            const string eventWithoutOrderString =
                "{\"seatTypeId\":\"783da597-49f8-0911-d5e8-0a0d7d83c704\",\"name\":\"Penningmeester\",\"version\":1,\"timestamp\":\"2017-09-21T07:44:13Z\"}";
            const string eventWithOrderZeroString =
                "{\"seatTypeId\":\"783da597-49f8-0911-d5e8-0a0d7d83c704\",\"name\":\"Penningmeester\", \"order\": 0,\"version\":1,\"timestamp\":\"2017-09-21T07:44:13Z\"}";
            const string eventWithOrderOneString =
                "{\"seatTypeId\":\"783da597-49f8-0911-d5e8-0a0d7d83c704\",\"name\":\"Penningmeester\", \"order\": 1,\"version\":1,\"timestamp\":\"2017-09-21T07:44:13Z\"}";

            var eventWithoutEntitled = JsonConvert.DeserializeObject<SeatTypeCreated>(eventWithoutOrderString);
            eventWithoutEntitled.Order.Should().Be(null);

            var eventWithEntitledFalse = JsonConvert.DeserializeObject<SeatTypeCreated>(eventWithOrderZeroString);
            eventWithEntitledFalse.Order.Should().Be(0);

            var eventWithEntitledTrue = JsonConvert.DeserializeObject<SeatTypeCreated>(eventWithOrderOneString);
            eventWithEntitledTrue.Order.Should().Be(1);
        }
    }
}
