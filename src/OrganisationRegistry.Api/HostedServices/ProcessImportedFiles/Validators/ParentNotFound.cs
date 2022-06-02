namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using SqlServer.Organisation;

public static class ParentNotFound
{
    public static ValidationIssue? Validate(ImmutableList<OrganisationListItem> organisationsCache, int rowNumber, DeserializedRecord record)
        => ValidationIssuesFactory.Create(rowNumber, CheckParent(organisationsCache, record).ToList(), FormatMessage);

    private static IEnumerable<string> CheckParent(ImmutableList<OrganisationListItem> organisationsCache, DeserializedRecord record)
    {
        if (record.Parent.Value is not { } parent || parent.IsNullOrWhiteSpace()) yield break;

        if (!organisationsCache.Any(org => org.OvoNumber.Equals(parent, StringComparison.InvariantCultureIgnoreCase)))
            yield return parent;
    }

    public static string FormatMessage(string parent)
        => $"Parent met Ovo nummer {parent} werd niet gevonden.";
}
