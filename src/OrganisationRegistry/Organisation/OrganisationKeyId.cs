namespace OrganisationRegistry.Organisation;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationKeyId : GuidValueObject<OrganisationKeyId>
{
    public OrganisationKeyId([JsonProperty("id")] Guid organisationKeyId) : base(organisationKeyId) { }
}