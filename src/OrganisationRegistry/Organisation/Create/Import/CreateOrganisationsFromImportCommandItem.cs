namespace OrganisationRegistry.Organisation.Import;

using System;
using System.Collections.Immutable;
using OrganisationRegistry.Organisation;

public record CreateOrganisationsFromImportCommandItem(string Reference, OrganisationParentIdentifier ParentIdentifier, string Name, int SortOrder)
{
    public DateOnly? Validity_Start { get; init; }
    public string? ShortName { get; init; }
    public Article? Article { get; init; }
    public DateOnly? OperationalValidity_Start { get; init; }
    public string? OvoNumber { get; init; }

    public ImmutableList<Label> Labels { get; init; } = ImmutableList<Label>.Empty;

    public CreateOrganisationsFromImportCommandItem WithOvoNumber(string ovoNumber)
        => this with { OvoNumber = ovoNumber };
}
