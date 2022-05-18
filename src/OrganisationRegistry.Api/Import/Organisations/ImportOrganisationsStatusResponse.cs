namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Collections.Generic;

public record ImportOrganisationStatusResponse(IEnumerable<ImportOganisationStatusResponseItem> Imports);

public record ImportOganisationStatusResponseItem(Guid Id, string FileName, string Status, string UploadedAt);
