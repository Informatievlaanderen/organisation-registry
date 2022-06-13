namespace OrganisationRegistry.Organisation.Import;

using System;
using System.Globalization;
using OrganisationRegistry.Organisation;

public record OutputRecord
{
    protected OutputRecord(string reference, OrganisationParentIdentifier parentIdentifier, string name, int sortOrder)
    {
        Reference = reference;
        ParentIdentifier = parentIdentifier;
        Name = name;
        SortOrder = sortOrder;
    }

    public string Reference { get; }
    public OrganisationParentIdentifier ParentIdentifier { get; }
    public string Name { get; }
    public DateOnly? Validity_Start { get; protected init; }
    public string? ShortName { get; protected init; }
    public Article? Article { get; protected init; }
    public DateOnly? OperationalValidity_Start { get; protected init; }
    public string? OvoNumber { get; private init; }
    public int SortOrder { get; }

    public static OutputRecord From(DeserializedRecord record, OrganisationParentIdentifier parentidentifier, int sortOrder)
        => new(record.Reference.Value!, parentidentifier, record.Name.Value!, sortOrder)
        {
            Article = Article.Parse(record.Article.Value),
            ShortName = record.ShortName.Value,
            Validity_Start = record.Validity_Start.Value is { } validityStart
                ? DateOnly.ParseExact(validityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null,
            OperationalValidity_Start = record.OperationalValidity_Start.Value is { } operationalValidityStart
                ? DateOnly.ParseExact(operationalValidityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null
        };

    public OutputRecord WithOvoNumber(string ovoNumber)
        => new(Reference, ParentIdentifier, Name, SortOrder)
        {
            Article = Article,
            ShortName = ShortName,
            Validity_Start = Validity_Start,
            OperationalValidity_Start = OperationalValidity_Start,
            OvoNumber = ovoNumber
        };
}
