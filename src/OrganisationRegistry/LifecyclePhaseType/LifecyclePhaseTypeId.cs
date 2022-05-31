namespace OrganisationRegistry.LifecyclePhaseType;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class LifecyclePhaseTypeId : GuidValueObject<LifecyclePhaseTypeId>
{
    public LifecyclePhaseTypeId([JsonProperty("id")] Guid lifecyclePhaseTypeId) : base(lifecyclePhaseTypeId) { }
}
