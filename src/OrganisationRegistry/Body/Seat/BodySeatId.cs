namespace OrganisationRegistry.Body;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class BodySeatId : GuidValueObject<BodySeatId>
{
    public BodySeatId([JsonProperty("id")] Guid bodySeatId) : base(bodySeatId) { }
}