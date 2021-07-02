namespace OrganisationRegistry.RegulationType
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class RegulationTypeId : GuidValueObject<RegulationTypeId>
    {
        public RegulationTypeId([JsonProperty("id")] Guid regulationTypeId) : base(regulationTypeId) { }
    }
}
