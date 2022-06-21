namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using ProcessImportedFiles.Validators;
using Validators;

public static class ImportRecordValidator
{
    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
        => parsedRecords
            .Aggregate(new ValidationIssues(), (aggregated, record) => aggregated.AddRange(ValidateParsedRecord(importCache, today, record)));

    private static IEnumerable<ValidationIssue> ValidateParsedRecord(ImportCache importCache, DateOnly today, ParsedRecord<DeserializedRecord> parsedRecord)
    {
        if (parsedRecord.ValidationIssues.Any())
            return parsedRecord.ValidationIssues;

        if (parsedRecord.OutputRecord is not { } outputRecord)
            throw new NullReferenceException("parsedRecord.OutputRecord should never be null");

        return ValidateRecord(importCache, today, parsedRecord.RowNumber, outputRecord).Items;
    }

    private static ValidationIssues ValidateRecord(ImportCache importCache, DateOnly today, int rowNumber, DeserializedRecord record)
        => new ValidationIssues()
            .Add(MissingRequiredFields.Validate(rowNumber, record.Reference, record.Parent, record.Name))
            .Add(InvalidArticle.Validate(rowNumber, record.Article))
            .AddRange(InvalidDateFormat.Validate(rowNumber, record.Validity_Start, record.OperationalValidity_Start))
            .Add(InvalidReference.Validate(rowNumber, record))
            .Add(ParentWithOvonumberNotFound.Validate(importCache.OrganisationsCache, rowNumber, record))
            .Add(ParentWithOvonumberValidityExpired.Validate(importCache.OrganisationsCache, today, rowNumber, record))
            .Add(ParentWithOvonumberAlreadyHasDaughterWithSameName.Validate(importCache.OrganisationsCache, rowNumber, record));
}
