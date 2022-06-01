namespace OrganisationRegistry.UnitTests.Body.Serialization;

using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.SeatType.Events;
using Xunit;

public class SeatTypeCreatedSerialization
{
    [Fact]
    public void CanDeserializeWithoutIsEffective()
    {
        const string @event =
            "{'seatTypeId':'cb37bf26-0942-40b2-a698-1d4afac1d5f1','name':'Toezichthouder','order':null,'version':1,'timestamp':'2018-06-18T12:57:48Z'}";
        var seatTypeCreated = JsonConvert.DeserializeObject<SeatTypeCreated>(@event);

        seatTypeCreated!.IsEffective.Should().BeNull();
    }

    [Fact]
    public void CanDeserializeWithIsEffective()
    {
        const string @event =
            "{'seatTypeId':'cb37bf26-0942-40b2-a698-1d4afac1d5f1','name':'Toezichthouder','order':null,'isEffective': true,'version':1,'timestamp':'2018-06-18T12:57:48Z'}";

        var bodyBalancedParticipationChanged =
            JsonConvert.DeserializeObject<SeatTypeCreated>(@event);

        bodyBalancedParticipationChanged!.IsEffective.Should().BeTrue();
    }
}
