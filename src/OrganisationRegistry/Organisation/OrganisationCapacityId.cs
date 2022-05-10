namespace OrganisationRegistry.Organisation;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationCapacityId : GuidValueObject<OrganisationCapacityId>
{
    public OrganisationCapacityId([JsonProperty("id")] Guid organisationCapacityId) : base(organisationCapacityId) { }
}
