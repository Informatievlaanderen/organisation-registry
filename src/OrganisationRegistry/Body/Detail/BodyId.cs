namespace OrganisationRegistry.Body;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class BodyId : GuidValueObject<BodyId>
{
    public BodyId([JsonProperty("id")] Guid bodyId) : base(bodyId) { }
}
