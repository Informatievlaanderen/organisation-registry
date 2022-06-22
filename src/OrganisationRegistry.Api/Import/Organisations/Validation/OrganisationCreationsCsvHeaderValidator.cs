namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;
using ColumnNames = HostedServices.ProcessImportedFiles.CreateOrganisations.ColumnNames;

public static class OrganisationCreationsCsvHeaderValidator
{
    public static CsvValidationResult Validate(ILogger logger, ImmutableList<string> validLabelTypes, string filename, string csvContent)
        => CsvHeaderValidator.Validate(
            logger,
            ColumnNames.Required,
            ColumnNames.Optional
                .AddRange(validLabelTypes.Select(labelType => $"label#{labelType.Trim().ToLowerInvariant()}")),
            filename,
            csvContent);
}
