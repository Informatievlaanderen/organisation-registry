namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations.Validators;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Validation;

public static class OvoNumberNotFound
{
    public static ValidationIssue? Validate(
        ImportCache organisationsCache,
        int rowNumber,
        Field ovoNumber)
        => ValidationIssuesFactory.Create(rowNumber, CheckOvoNumber(organisationsCache, ovoNumber).ToList(), FormatMessage);

    private static IEnumerable<string> CheckOvoNumber(
        ImportCache organisationsCache,
        Field field)
    {
        if (field.Value is not { } ovoNumber || ovoNumber.IsNullOrWhiteSpace()) yield break;

        if (organisationsCache.GetOrganisationByOvoNumber(ovoNumber) is not { })
            yield return ovoNumber;
    }

    public static string FormatMessage(string ovoNumber)
        => $"Ovo nummer {ovoNumber} werd niet gevonden.";
}
