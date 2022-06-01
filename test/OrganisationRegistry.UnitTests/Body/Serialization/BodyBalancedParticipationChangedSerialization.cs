namespace OrganisationRegistry.UnitTests.Body.Serialization;

using FluentAssertions;
using Newtonsoft.Json;
using OrganisationRegistry.Body.Events;
using Xunit;

public class BodyBalancedParticipationChangedSerialization
{
    [Fact]
    public void CanDeserializeWithTrue()
    {
        const string @event =
            "{'bodyId':'9ca2e408-c1e0-113c-0e44-25db49cf6d1d','balancedParticipationObligatory':true,'balancedParticipationExtraRemark':'Aangeduid op 27/09/2019','balancedParticipationExceptionMeasure':null,'previousBalancedParticipationObligatory':false,'previousBalancedParticipationExtraRemark':null,'previousBalancedParticipationExceptionMeasure':null,'version':37,'timestamp':'2019-09-27T08:09:58Z'}";

        var bodyBalancedParticipationChanged =
            JsonConvert.DeserializeObject<BodyBalancedParticipationChanged>(@event);

        bodyBalancedParticipationChanged!.BalancedParticipationObligatory.Should().BeTrue();
    }

    [Fact]
    public void CanDeserializeWithFalse()
    {
        const string @event =
            "{'bodyId':'9ca2e408-c1e0-113c-0e44-25db49cf6d1d','balancedParticipationObligatory':false,'balancedParticipationExtraRemark':'Aangeduid op 27/09/2019','balancedParticipationExceptionMeasure':null,'previousBalancedParticipationObligatory':false,'previousBalancedParticipationExtraRemark':null,'previousBalancedParticipationExceptionMeasure':null,'version':37,'timestamp':'2019-09-27T08:09:58Z'}";

        var bodyBalancedParticipationChanged =
            JsonConvert.DeserializeObject<BodyBalancedParticipationChanged>(@event);

        bodyBalancedParticipationChanged!.BalancedParticipationObligatory.Should().BeFalse();
    }
}
