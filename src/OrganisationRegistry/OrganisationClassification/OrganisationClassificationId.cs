namespace OrganisationRegistry.OrganisationClassification;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationClassificationId : GuidValueObject<OrganisationClassificationId>
{
    public OrganisationClassificationId([JsonProperty("id")] Guid organisationClassificationId) : base(organisationClassificationId) { }
}