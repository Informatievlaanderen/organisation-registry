namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System;
using System.Globalization;
using Organisation.Import;

public record DeserializedRecord
{
    public Field OvoNumber { get; init; } = Field.NoValue(ColumnNames.OvoNumber);
    public Field Name { get; init; } = Field.NoValue(ColumnNames.Name);
    public Field Organisation_End { get; init; } = Field.NoValue(ColumnNames.Organisation_End);

    public StopOrganisationsFromImportCommandItem ToOutputRecord(Guid organisationId)
        => new(organisationId, GetDate(Organisation_End.Value));

    private static DateOnly GetDate(string? date)
        => DateOnly.ParseExact(date!, "yyyy-MM-dd", CultureInfo.InvariantCulture);
}
