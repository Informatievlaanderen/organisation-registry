namespace OrganisationRegistry.FormalFrameworkCategory;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class FormalFrameworkCategoryId : GuidValueObject<FormalFrameworkCategoryId>
{
    public FormalFrameworkCategoryId([JsonProperty("id")] Guid formalFrameworkCategoryId) : base(formalFrameworkCategoryId) { }
}