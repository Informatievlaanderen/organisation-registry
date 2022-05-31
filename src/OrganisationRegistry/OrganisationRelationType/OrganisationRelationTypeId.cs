namespace OrganisationRegistry.OrganisationRelationType;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationRelationTypeId : GuidValueObject<OrganisationRelationTypeId>
{
    public OrganisationRelationTypeId([JsonProperty("id")] Guid organisationRelationTypeId) : base(organisationRelationTypeId) { }
}
