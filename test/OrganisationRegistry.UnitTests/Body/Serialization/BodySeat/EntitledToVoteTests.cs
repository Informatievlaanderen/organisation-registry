namespace OrganisationRegistry.UnitTests.Body.Serialization.BodySeat;

using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.Body.Events;
using Xunit;

public class EntitledToVoteTests
{
    [Fact]
    public void CanDeserializeWithAndWithoutEntitledString()
    {
        const string eventWithoutEntitledString =
            "{\"bodyId\":\"18cf8a93-0aaf-af55-f7cc-00587d8e31f5\",\"bodySeatId\":\"a88564a0-f0fd-0e7b-926b-b4d1f8b116f0\",\"name\":\"Voorzitter\",\"bodySeatNumber\":\"POS003350\",\"seatTypeId\":\"a49f3389-3bc4-59ab-d690-b38e0f7b1910\",\"seatTypeName\":\"Voorzitter\",\"paidSeat\":false,\"validFrom\":null,\"validTo\":null,\"version\":7,\"timestamp\":\"2017-07-25T06:46:50Z\"}";
        const string eventWithEntitledFalseString =
            "{\"bodyId\":\"18cf8a93-0aaf-af55-f7cc-00587d8e31f5\",\"bodySeatId\":\"a88564a0-f0fd-0e7b-926b-b4d1f8b116f0\",\"name\":\"Voorzitter\",\"bodySeatNumber\":\"POS003350\",\"seatTypeId\":\"a49f3389-3bc4-59ab-d690-b38e0f7b1910\",\"seatTypeName\":\"Voorzitter\",\"paidSeat\":false,\"entitledToVote\":false,\"validFrom\":null,\"validTo\":null,\"version\":7,\"timestamp\":\"2017-07-25T06:46:50Z\"}";
        const string eventWithEntitledTrueString =
            "{\"bodyId\":\"18cf8a93-0aaf-af55-f7cc-00587d8e31f5\",\"bodySeatId\":\"a88564a0-f0fd-0e7b-926b-b4d1f8b116f0\",\"name\":\"Voorzitter\",\"bodySeatNumber\":\"POS003350\",\"seatTypeId\":\"a49f3389-3bc4-59ab-d690-b38e0f7b1910\",\"seatTypeName\":\"Voorzitter\",\"paidSeat\":false,\"entitledToVote\":true,\"validFrom\":null,\"validTo\":null,\"version\":7,\"timestamp\":\"2017-07-25T06:46:50Z\"}";

        var eventWithoutEntitled = JsonConvert.DeserializeObject<BodySeatAdded>(eventWithoutEntitledString);
        eventWithoutEntitled!.EntitledToVote.Should().BeFalse();

        var eventWithEntitledFalse = JsonConvert.DeserializeObject<BodySeatAdded>(eventWithEntitledFalseString);
        eventWithEntitledFalse!.EntitledToVote.Should().BeFalse();

        var eventWithEntitledTrue = JsonConvert.DeserializeObject<BodySeatAdded>(eventWithEntitledTrueString);
        eventWithEntitledTrue!.EntitledToVote.Should().BeTrue();
    }
}