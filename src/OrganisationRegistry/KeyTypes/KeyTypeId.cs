namespace OrganisationRegistry.KeyTypes;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class KeyTypeId : GuidValueObject<KeyTypeId>
{
    public KeyTypeId([JsonProperty("id")] Guid keyTypeId) : base(keyTypeId) { }
}
