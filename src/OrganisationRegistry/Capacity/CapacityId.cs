namespace OrganisationRegistry.Capacity;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class CapacityId : GuidValueObject<CapacityId>
{
    public CapacityId([JsonProperty("id")] Guid capacityId) : base(capacityId) { }
}