namespace OrganisationRegistry.LabelType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class LabelTypeId : GuidValueObject<LabelTypeId>
    {
        public LabelTypeId([JsonProperty("id")] Guid labelTypeId) : base(labelTypeId) { }
    }
}
