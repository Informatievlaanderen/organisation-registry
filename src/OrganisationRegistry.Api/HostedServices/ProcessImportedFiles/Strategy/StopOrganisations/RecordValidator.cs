namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using ProcessImportedFiles.Validators;
using Validators;

public static class RecordValidator
{
    public static ValidationIssues Validate(ImportCache importCache, IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Aggregate(new ValidationIssues(), (aggregated, record) => aggregated.AddRange(ValidateParsedRecord(importCache, record)));

    private static IEnumerable<ValidationIssue> ValidateParsedRecord(ImportCache importCache, ParsedRecord parsedRecord)
    {
        if (parsedRecord.ValidationIssues.Any())
            return parsedRecord.ValidationIssues;

        if (parsedRecord.OutputRecord is not { } outputRecord)
            throw new NullReferenceException("parsedRecord.OutputRecord should never be null");

        return ValidateRecord(importCache, parsedRecord.RowNumber, outputRecord).Items;
    }

    private static ValidationIssues ValidateRecord(ImportCache importCache, int rowNumber, DeserializedRecord record)
        => new ValidationIssues()
            .Add(MissingRequiredFields.Validate(rowNumber, record.OvoNumber, record.Name, record.Organisation_End))
            .Add(OvoNumberNotFound.Validate(importCache, rowNumber, record.OvoNumber))
            .Add(OvoNumberDoesNotMatchWithName.Validate(importCache, rowNumber, record))
            .Add(OrganisationContainsKboNumber.Validate(importCache, rowNumber, record.OvoNumber))
            .AddRange(InvalidDateFormat.Validate(rowNumber, record.Organisation_End));
}
