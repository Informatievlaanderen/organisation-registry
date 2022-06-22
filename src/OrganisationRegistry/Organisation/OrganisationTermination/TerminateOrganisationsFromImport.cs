namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Import;

public class TerminateOrganisationsFromImport : BaseCommand<OrganisationSourceId>
{
    public Guid ImportFileId { get; }
    public IEnumerable<TerminateOrganisationsFromImportCommandItem> Records { get; }

    public TerminateOrganisationsFromImport(Guid importFileId, IEnumerable<TerminateOrganisationsFromImportCommandItem> records)
    {
        ImportFileId = importFileId;
        Records = records;
    }
}
