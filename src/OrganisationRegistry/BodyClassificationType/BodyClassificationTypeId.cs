namespace OrganisationRegistry.BodyClassificationType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class BodyClassificationTypeId : GuidValueObject<BodyClassificationTypeId>
    {
        public BodyClassificationTypeId([JsonProperty("id")] Guid bodyClassificationTypeId) : base(bodyClassificationTypeId) { }
    }
}
