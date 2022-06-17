namespace OrganisationRegistry.Organisation.Import;

using System;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using OrganisationRegistry.Organisation;

public record OutputRecord
{
    private OutputRecord(string reference, OrganisationParentIdentifier parentIdentifier, string name, int sortOrder)
    {
        Reference = reference;
        ParentIdentifier = parentIdentifier;
        Name = name;
        SortOrder = sortOrder;
    }

    public string Reference { get; }
    public OrganisationParentIdentifier ParentIdentifier { get; }
    public string Name { get; }
    public DateOnly? Validity_Start { get; private init; }
    public string? ShortName { get; private init; }
    public Article? Article { get; private init; }
    public DateOnly? OperationalValidity_Start { get; private init; }
    public string? OvoNumber { get; private init; }
    public int SortOrder { get; }

    public ImmutableList<Label> Labels { get; private init; } = ImmutableList<Label>.Empty;

    public static OutputRecord From(DeserializedRecord record, OrganisationParentIdentifier parentidentifier, int sortOrder)
        => new(record.Reference.Value!, parentidentifier, record.Name.Value!, sortOrder)
        {
            Article = Article.Parse(record.Article.Value),
            ShortName = record.ShortName.Value,
            Validity_Start = MaybeGetDate(record.Validity_Start.Value),
            OperationalValidity_Start = MaybeGetDate(record.OperationalValidity_Start.Value),
            Labels = GetLabels(record)
        };

    private static ImmutableList<Label> GetLabels(DeserializedRecord record)
        => record.Labels
            .Select(label => new Label(label.ColumnName.Split('#')[1], label.Value!))
            .ToImmutableList();

    private static DateOnly? MaybeGetDate(string? maybeDate)
        => maybeDate is { } date
            ? DateOnly.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture)
            : null;

    public OutputRecord WithOvoNumber(string ovoNumber)
        => this with { OvoNumber = ovoNumber };
}
