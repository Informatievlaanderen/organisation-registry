namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using Organisation.Import;
using Validators;

public static class RecordValidator
{
    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Aggregate(new ValidationIssues(), (aggregated, record) => aggregated.AddRange(ValidateParsedRecord(importCache, today, record)));

    private static IEnumerable<ValidationIssue> ValidateParsedRecord(ImportCache importCache, DateOnly today, ParsedRecord parsedRecord)
    {
        if (parsedRecord.ValidationIssues.Any())
            return parsedRecord.ValidationIssues;

        if (parsedRecord.OutputRecord is not { } outputRecord)
            throw new NullReferenceException("parsedRecord.OutputRecord should never be null");

        return ValidateRecord(importCache, today, parsedRecord.RowNumber, outputRecord).Items;
    }

    private static ValidationIssues ValidateRecord(ImportCache importCache, DateOnly today, int rowNumber, DeserializedRecord record)
        => new ValidationIssues()
            .Add(MissingRequiredFields.Validate(rowNumber, record))
            .Add(InvalidArticle.Validate(rowNumber, record))
            .AddRange(InvalidDateFormat.Validate(rowNumber, record))
            .Add(InvalidReference.Validate(rowNumber, record))
            .Add(ParentNotFound.Validate(importCache.OrganisationsCache, rowNumber, record))
            .Add(ParentValidityExpired.Validate(importCache.OrganisationsCache, today, rowNumber, record))
            .Add(ParentAlreadyHasDaughterWithSameName.Validate(importCache.OrganisationsCache, rowNumber, record));
}
