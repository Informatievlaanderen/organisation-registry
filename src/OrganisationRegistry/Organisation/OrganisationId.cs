namespace OrganisationRegistry.Organisation;

using System;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class OrganisationId : GuidValueObject<OrganisationId>
{
    public OrganisationId([JsonProperty("id")] Guid organisationId) : base(organisationId)
    {
    }

    public static OrganisationId New()
        => new(Guid.NewGuid());
}
