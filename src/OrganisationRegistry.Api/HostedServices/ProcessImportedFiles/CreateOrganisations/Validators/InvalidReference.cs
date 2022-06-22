namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations.Validators;

using System.Collections.Generic;
using System.Linq;
using Validation;

public static class InvalidReference
{
    public static ValidationIssue? Validate(int rowNumber, DeserializedRecord record)
        => ValidationIssuesFactory.Create(rowNumber, CheckInvalidArticle(record).ToList(), FormatMessage);

    private static IEnumerable<string> CheckInvalidArticle(DeserializedRecord record)
    {
        if (record.Reference.Value is not { } reference) yield break;

        if (reference.ToLowerInvariant().StartsWith("ovo"))
            yield return reference;
    }

    public static string FormatMessage(string reference)
        => $"De waarde {reference} is ongeldig voor kolom 'reference' (Waarde mag niet beginnen met 'OVO').";
}
