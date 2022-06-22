namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations.Validators;

using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Validation;

public static class OrganisationContainsKboNumber
{
    public static ValidationIssue? Validate(
        ImportCache organisationsCache,
        int rowNumber,
        Field field)
    {
        if (field.Value is not { } ovoNumber || ovoNumber.IsNullOrWhiteSpace()) return null;

        if (organisationsCache.GetOrganisationByOvoNumber(ovoNumber) is not { } organisation)
            return null;

        return organisation.KboNumber is not { } kboNumber
            ? null
            : new ValidationIssue(rowNumber, FormatMessage(ovoNumber, kboNumber));
    }

    public static string FormatMessage(string ovoNumber, string kboNumber)
        => $"Organisatie met ovo nummer {ovoNumber} heeft een kbo nummer ({kboNumber}). Dit is niet toegelaten.";
}
