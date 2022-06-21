namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using Import;

public class CreateOrganisationsFromImport : BaseCommand<OrganisationSourceId>
{
    public Guid ImportFileId { get; }
    public IEnumerable<CreateOrganisationsFromImportCommandItem> Records { get; }

    public CreateOrganisationsFromImport(Guid importFileId, IEnumerable<CreateOrganisationsFromImportCommandItem> records)
    {
        ImportFileId = importFileId;
        Records = records;
    }
}
