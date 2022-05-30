namespace OrganisationRegistry.Body;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class BodyOrganisationId : GuidValueObject<BodyOrganisationId>
{
    public BodyOrganisationId([JsonProperty("id")] Guid bodyOrganisationId) : base(bodyOrganisationId) { }
}