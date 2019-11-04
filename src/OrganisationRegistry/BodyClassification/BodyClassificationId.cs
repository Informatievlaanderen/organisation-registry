namespace OrganisationRegistry.BodyClassification
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class BodyClassificationId : GuidValueObject<BodyClassificationId>
    {
        public BodyClassificationId([JsonProperty("id")] Guid bodyClassificationId) : base(bodyClassificationId) { }
    }
}
