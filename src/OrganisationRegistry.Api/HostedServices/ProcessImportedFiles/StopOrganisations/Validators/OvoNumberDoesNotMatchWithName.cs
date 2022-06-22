namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations.Validators;

using System;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Validation;

public static class OvoNumberDoesNotMatchWithName
{
    public static ValidationIssue? Validate(
        ImportCache organisationsCache,
        int rowNumber,
        DeserializedRecord record)
    {
        if (record.OvoNumber.Value is not { } ovoNumber || ovoNumber.IsNullOrWhiteSpace()) return null;

        if (organisationsCache.GetOrganisationByOvoNumber(ovoNumber) is not { } organisation)
            return null;

        if (!record.Name.HasValue)
            return null;

        return string.Equals(organisation.Name, record.Name.Value, StringComparison.InvariantCultureIgnoreCase)
            ? null
            : new ValidationIssue(rowNumber, FormatMessage(record.OvoNumber.Value, record.Name.Value));
    }

    public static string FormatMessage(string ovoNumber, string? name)
        => $"Ovo nummer '{ovoNumber}' komt niet overeen met organisatie naam '{name}'.";
}
