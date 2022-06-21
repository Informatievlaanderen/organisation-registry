namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.Extensions.Logging;

public static class CsvHeaderValidator
{
    public static CsvValidationResult Validate(ILogger logger, ImmutableList<string> requiredColumnNames, ImmutableList<string> optionalColumnNames, string filename, string csvContent)
    {
        try
        {
            using var reader = new StringReader(csvContent);
            using var csv = new CsvReader(
                reader,
                new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

            csv.Read();
            csv.ReadHeader();

            while (csv.Read())
            {
            }

            var csvHeaderRecord = csv.HeaderRecord
                .Select(columnName => columnName.Trim().ToLower())
                .ToImmutableList();

            return new ValidationIssues()
                .Add(InvalidFilename.Validate(filename))
                .Add(MissingRequiredColumns.Validate(csvHeaderRecord, requiredColumnNames))
                .Add(DuplicateColumns.Validate(csvHeaderRecord))
                .Add(InvalidColumns.Validate(csvHeaderRecord, requiredColumnNames.AddRange(optionalColumnNames)))
                .ToResult();
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error occured when validating imported CSV file {File}: {Message}",
                filename,
                ex.Message);
            return new ValidationIssues().Add(
                new ValidationIssue(
                    $"Het bestand {filename} kon niet verwerkt worden. \n" +
                    "Gelieve het formaat te controleren. \n" +
                    "Contacteer de beheerders indien de fout zich opnieuw voordoet."))
                .ToResult();
        }
    }
}
