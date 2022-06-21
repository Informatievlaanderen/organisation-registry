namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Import;

public class TerminateOrganisationsFromImport : BaseCommand<OrganisationSourceId>
{
    public Guid ImportFileId { get; }
    public IEnumerable<StopOrganisationsFromImportCommandItem> Records { get; }

    public TerminateOrganisationsFromImport(Guid importFileId, IEnumerable<StopOrganisationsFromImportCommandItem> records)
    {
        ImportFileId = importFileId;
        Records = records;
    }
}
