namespace OrganisationRegistry.Function;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class FunctionTypeId : GuidValueObject<FunctionTypeId>
{
    public FunctionTypeId([JsonProperty("id")] Guid functionTypeId) : base(functionTypeId) { }
}