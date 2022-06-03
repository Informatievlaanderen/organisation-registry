namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System;
using System.Collections.Immutable;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using SqlServer.Organisation;

public static class ParentAlreadyHasDaughterWithSameName
{
    public static ValidationIssue? Validate(ImmutableList<OrganisationListItem> organisationsCache, int rowNumber, DeserializedRecord record)
    {
        if (record.Parent.Value is not { } parent || parent.IsNullOrWhiteSpace())
            return null;

        return organisationsCache
            .Where(org => org.Name.Equals(record.Name.Value, StringComparison.InvariantCultureIgnoreCase))
            .Where(org => org.ParentOrganisationOvoNumber != null)
            .Any(org => org.ParentOrganisationOvoNumber == record.Parent.Value)
            ? new ValidationIssue(rowNumber, FormatMessage(parent, record.Name.Value))
            : null;
    }

    public static string FormatMessage(string parent, string? name)
        => $"Parent met Ovo nummer '{parent}' heeft al een dochter met naam '{name}'.";
}
