namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

public static class ImportOrganisationCsvHeaderValidator
{
    private static readonly ImmutableList<string> RequiredColumnNames =
        ImmutableList.Create<string>("reference", "parent", "name");

    private static readonly ImmutableList<string> OptionalColumnNames = ImmutableList.Create<string>(
        "validity_start",
        "shortname",
        "article",
        "operationalvalidity_start");

    public static CsvValidationResult Validate(ILogger logger, ImmutableList<string> validLabelTypes, string filename, string csvContent)
        => CsvHeaderValidator.Validate(
            logger,
            RequiredColumnNames,
            OptionalColumnNames.AddRange(validLabelTypes.Select(labelType => $"label#{labelType.Trim().ToLowerInvariant()}")),
            filename,
            csvContent);
}

public static class ImportStopOrganisationCsvHeaderValidator
{
    private static readonly ImmutableList<string> RequiredColumnNames =
        ImmutableList.Create<string>("ovonumber", "name", "organisations_end");

    private static readonly ImmutableList<string> OptionalColumnNames = ImmutableList<string>.Empty;

    public static CsvValidationResult Validate(ILogger logger, string filename, string csvContent)
        => CsvHeaderValidator.Validate(
            logger,
            RequiredColumnNames,
            OptionalColumnNames,
            filename,
            csvContent);
}
