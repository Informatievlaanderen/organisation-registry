namespace OrganisationRegistry.Organisation;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationSourceId : GuidValueObject<OrganisationSourceId>
{
    public OrganisationSourceId([JsonProperty("id")] Guid organisationId) : base(organisationId)
    {
    }

    public static implicit operator Guid(OrganisationSourceId valueObject)
        => valueObject.Value;

    public static implicit operator Guid?(OrganisationSourceId? valueObject)
        => valueObject?.Value;
}
