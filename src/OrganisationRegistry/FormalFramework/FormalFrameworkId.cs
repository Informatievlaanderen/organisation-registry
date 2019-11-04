namespace OrganisationRegistry.FormalFramework
{
    using System;
    using Be.Vlaanderen.Basisregisters.AggregateSource;
    using Newtonsoft.Json;

    public class FormalFrameworkId : GuidValueObject<FormalFrameworkId>
    {
        public FormalFrameworkId([JsonProperty("id")] Guid formalFrameworkId) : base(formalFrameworkId) { }
    }
}
