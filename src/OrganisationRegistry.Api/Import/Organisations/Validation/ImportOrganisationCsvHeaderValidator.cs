namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

public static class ImportOrganisationCsvHeaderValidator
{
    private static readonly ImmutableList<string> RequiredColumnNames = ImmutableList.Create<string>("reference", "parent", "name");
    private static readonly ImmutableList<string> OptionalColumnNames = ImmutableList.Create<string>("validity_start", "shortname", "article", "operationalvalidity_start");

    public static CsvValidationResult Validate(ILogger logger, string filename, string csvContent)
    {
        var validationIssues = new ValidationIssues();
        try
        {
            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

            csv.Read();
            csv.ReadHeader();

            var csvHeaderRecord = csv.HeaderRecord
                .Select(columnName => columnName.Trim().ToLower())
                .ToImmutableList();

            validationIssues = validationIssues.Add(InvalidFilename.Validate(filename));
            validationIssues = validationIssues.Add(MissingRequiredColumns.Validate(csvHeaderRecord, RequiredColumnNames));
            validationIssues = validationIssues.Add(DuplicateColumns.Validate(csvHeaderRecord));
            validationIssues = validationIssues.Add(InvalidColumns.Validate(csvHeaderRecord, RequiredColumnNames.AddRange(OptionalColumnNames)));
        }
        catch (Exception ex)
        {
            validationIssues = validationIssues.Add(new ValidationIssue($"Het bestand {filename} kon niet verwerkt worden. \n" +
                                                                        "Gelieve het formaat te controleren. \n" +
                                                                        "Contacteer de beheerders indien de fout zich opnieuw voordoet."));
            logger.LogError(ex, "Error occured when validating imported CSV file {File}: {Message}", filename, ex.Message);
        }

        return validationIssues.ToResult();
    }
}
