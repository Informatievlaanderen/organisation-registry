namespace OrganisationRegistry.Body;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class BodyMandateId : GuidValueObject<BodyMandateId>
{
    public BodyMandateId([JsonProperty("id")] Guid bodyMandateId) : base(bodyMandateId) { }
}
