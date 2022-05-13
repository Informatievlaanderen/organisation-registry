namespace OrganisationRegistry.Body;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class BodyFormalFrameworkId : GuidValueObject<BodyFormalFrameworkId>
{
    public BodyFormalFrameworkId([JsonProperty("id")] Guid bodyFormalFrameworkId) : base(bodyFormalFrameworkId) { }
}
