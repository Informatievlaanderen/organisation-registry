namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
using Newtonsoft.Json;

public record ImportOrganisationStatusResponse(IEnumerable<ImportOrganisationStatusResponseItem> Imports);

public class ImportOrganisationStatusResponseItem
{
    public ImportOrganisationStatusResponseItem(Guid id, string status, string fileName, DateTimeOffset uploadedAt, string importFileType)
    {
        Id = id;
        FileName = fileName;
        Status = status;
        UploadedAt = uploadedAt;
        ImportFileType = importFileType;
    }

    public Guid Id { get; init; }
    public string FileName { get; init; }
    public string Status { get; init; }

    [JsonConverter(typeof(TimestampConverter))]
    public DateTimeOffset UploadedAt { get; init; }

    public string ImportFileType { get; init; }
}
