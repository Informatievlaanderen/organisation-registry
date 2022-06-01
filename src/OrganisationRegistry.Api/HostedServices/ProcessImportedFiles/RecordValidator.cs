namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using Validators;

public static class RecordValidator
{
    public static ValidationIssues Validate(IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Aggregate(new ValidationIssues(), (aggregated, record) => aggregated.AddRange(ValidateParsedRecord(record)));

    private static IEnumerable<ValidationIssue> ValidateParsedRecord(ParsedRecord parsedRecord)
    {
        if (parsedRecord.ValidationIssues.Any())
            return parsedRecord.ValidationIssues;

        if (parsedRecord.OutputRecord is not { } outputRecord)
            throw new NullReferenceException("parsedRecord.OutputRecord should never be null");

        return ValidateRecord(parsedRecord.RowNumber, outputRecord).Items;
    }

    private static ValidationIssues ValidateRecord(int rowNumber, DeserializedRecord record)
        => new ValidationIssues()
            .Add(MissingRequiredFields.Validate(rowNumber, record))
            .Add(InvalidArticle.Validate(rowNumber, record))
            .AddRange(InvalidDateFormat.Validate(rowNumber, record))
            .Add(InvalidReference.Validate(rowNumber, record));
}
