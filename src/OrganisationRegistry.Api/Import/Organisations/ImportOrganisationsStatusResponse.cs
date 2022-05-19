namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public record ImportOrganisationStatusResponse(IEnumerable<ImportOganisationStatusResponseItem> Imports);

public class ImportOganisationStatusResponseItem
{
    public ImportOganisationStatusResponseItem(Guid id, string status, string fileName, DateTimeOffset uploadedAt)
    {
        Id = id;
        FileName = fileName;
        Status = status;
        UploadedAt = uploadedAt;
    }

    public Guid Id { get; init; }
    public string FileName { get; init; }
    public string Status { get; init; }
    [JsonConverter(typeof(TimestampConverter))]
    public DateTimeOffset UploadedAt { get; init; }

}
