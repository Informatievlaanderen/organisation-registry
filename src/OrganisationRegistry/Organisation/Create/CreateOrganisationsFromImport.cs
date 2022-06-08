namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;
using Newtonsoft.Json;

public class CreateOrganisationsFromImport : BaseCommand<ImportId>
{
    public Guid ImportFileId { get; }
    public IEnumerable<IOutputRecord> Records { get; }

    public CreateOrganisationsFromImport(Guid importFileId, IEnumerable<IOutputRecord> records)
    {
        ImportFileId = importFileId;
        Records = records;
    }
}

public class ImportId: GuidValueObject<ImportId>
{
    public ImportId([JsonProperty("id")] Guid organisationId) : base(organisationId) { }
}
