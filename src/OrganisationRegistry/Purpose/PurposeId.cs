namespace OrganisationRegistry.Purpose;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class PurposeId : GuidValueObject<PurposeId>
{
    public PurposeId([JsonProperty("id")] Guid purposeId) : base(purposeId) { }
}
