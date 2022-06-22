namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using HostedServices.ProcessImportedFiles.StopOrganisations;
using Microsoft.Extensions.Logging;

public static class ImportOrganisationTerminationsCsvHeaderValidator
{
    public static CsvValidationResult Validate(ILogger logger, string filename, string csvContent)
        => CsvHeaderValidator.Validate(
            logger,
            ColumnNames.Required,
            ColumnNames.Optional,
            filename,
            csvContent);
}
