namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using SqlServer.Organisation;

public static class ParentValidityExpired
{
    public static ValidationIssue? Validate(ImmutableList<OrganisationListItem> organisationsCache, DateOnly today, int rowNumber, DeserializedRecord record)
        => ValidationIssuesFactory.Create(rowNumber, CheckParent(organisationsCache, today, record).ToList(), FormatMessage);

    private static IEnumerable<string> CheckParent(ImmutableList<OrganisationListItem> organisationsCache, DateOnly today, DeserializedRecord record)
    {
        if (record.Parent.Value is not { } parent || parent.IsNullOrWhiteSpace()) yield break;

        if (organisationsCache.SingleOrDefault(org => org.OvoNumber.Equals(parent, StringComparison.InvariantCultureIgnoreCase)) is not { } parentOrganisation)
            yield break;

        if (parentOrganisation.ValidTo is { } validTo && validTo < today.ToDateTime(new TimeOnly()))
            yield return parent;
    }

    public static string FormatMessage(string parent)
        => $"Parent met Ovo nummer {parent} is reeds afgesloten.";
}
