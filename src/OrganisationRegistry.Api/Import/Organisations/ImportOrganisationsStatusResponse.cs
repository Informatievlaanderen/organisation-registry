namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Collections.Generic;

public class ImportOrganisationStatusResponse
{
    public IEnumerable<ImportOganisationStatusResponseItem> Imports { get; set; }
}

public class ImportOganisationStatusResponseItem
{
    public Guid Id { get; set; }
    public string FileName { get; set; }
    public string Status { get; set; }
    public DateTimeOffset UploadedAt { get; set; }
}
