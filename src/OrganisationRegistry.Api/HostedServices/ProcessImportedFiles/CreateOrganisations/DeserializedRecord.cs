namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using OrganisationRegistry.Organisation.Import;

public record DeserializedRecord
{
    public Field Reference { get; init; } = Field.NoValue(ColumnNames.Reference);
    public Field Parent { get; init; } = Field.NoValue(ColumnNames.Parent);
    public Field Name { get; init; } = Field.NoValue(ColumnNames.Name);
    public Field Validity_Start { get; init; } = Field.NoValue(ColumnNames.Validity_Start);
    public Field ShortName { get; init; } = Field.NoValue(ColumnNames.ShortName);
    public Field Article { get; init; } = Field.NoValue(ColumnNames.Article);
    public Field OperationalValidity_Start { get; init; } = Field.NoValue(ColumnNames.OperationalValidity_Start);
    public ImmutableList<Field> Labels { get; init; } = ImmutableList<Field>.Empty;

    public CreateOrganisationsFromImportCommandItem ToCommandItem(Dictionary<string, (Guid id, string name)> labelTypes, OrganisationParentIdentifier parentidentifier, int sortOrder)
        => new(Reference.Value!, parentidentifier, Name.Value!, sortOrder)
        {
            Article = Organisation.Article.Parse(Article.Value),
            ShortName = ShortName.Value,
            Validity_Start = MaybeGetDate(Validity_Start.Value),
            OperationalValidity_Start = MaybeGetDate(OperationalValidity_Start.Value),
            Labels = GetLabels(labelTypes, this),
        };

    private static ImmutableList<Label> GetLabels(IReadOnlyDictionary<string, (Guid id, string name)> labelTypes, DeserializedRecord record)
        => record.Labels
            .Select(label => CreateLabel(labelTypes, label.ColumnName.Split('#')[1], label.Value!))
            .ToImmutableList();

    private static Label CreateLabel(IReadOnlyDictionary<string, (Guid id, string name)> labelTypes, string labelTypeName, string labelValue)
        => new(labelTypes[labelTypeName].id, labelTypes[labelTypeName].name, labelValue);

    private static DateOnly? MaybeGetDate(string? maybeDate)
        => maybeDate is { } date
            ? DateOnly.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            : null;
}
