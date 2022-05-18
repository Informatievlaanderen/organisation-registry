namespace OrganisationRegistry.Body
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class BodyLifecyclePhaseId : GuidValueObject<BodyLifecyclePhaseId>
    {
        public BodyLifecyclePhaseId([JsonProperty("id")] Guid bodyLifecyclePhaseId) : base(bodyLifecyclePhaseId) { }
    }
}
