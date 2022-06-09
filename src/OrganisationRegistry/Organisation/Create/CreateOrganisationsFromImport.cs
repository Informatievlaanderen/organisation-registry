namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Import;
using Newtonsoft.Json;

public class CreateOrganisationsFromImport : BaseCommand<OrganisationSourceId>
{
    public Guid ImportFileId { get; }
    public IEnumerable<OutputRecord> Records { get; }

    public CreateOrganisationsFromImport(Guid importFileId, IEnumerable<OutputRecord> records)
    {
        ImportFileId = importFileId;
        Records = records;
    }
}

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
