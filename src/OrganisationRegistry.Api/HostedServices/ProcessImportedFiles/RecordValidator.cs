namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using Validators;

public static class RecordValidator
{
    public static IEnumerable<ValidationIssue> Validate(IEnumerable<ParsedRecord> parsedRecords)
    {
        var validationIssues = new ValidationIssues();
        validationIssues = parsedRecords.Aggregate(validationIssues, (current, parsedRecord) => current.AddRange(ValidateParsedRecord(parsedRecord)));
        return validationIssues.Items;
    }

    private static IEnumerable<ValidationIssue> ValidateParsedRecord(ParsedRecord parsedRecord)
    {
        if (parsedRecord.ValidationIssues.Any())
        {
            return parsedRecord.ValidationIssues;
        }

        if (parsedRecord.OutputRecord is not { } outputRecord)
        {
            //TODO add validation issue to result
            throw new Exception();
        }

        return ValidateRecord(parsedRecord.RowNumber, outputRecord).Items;
    }

    private static ValidationIssues ValidateRecord(int rowNumber, DeserializedRecord record)
    {
        var validationIssues = new ValidationIssues();

        validationIssues = validationIssues.Add(MissingRequiredFields.Validate(rowNumber, record));
        validationIssues = validationIssues.Add(InvalidArticle.Validate(rowNumber, record));
        validationIssues = validationIssues.AddRange(InvalidDateFormat.Validate(rowNumber, record));
        validationIssues = validationIssues.Add(InvalidReference.Validate(rowNumber, record));

        return validationIssues;
    }
}
