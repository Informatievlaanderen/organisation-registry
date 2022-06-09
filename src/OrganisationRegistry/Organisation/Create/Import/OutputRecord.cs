namespace OrganisationRegistry.Organisation.Import;

using System;
using System.Globalization;
using OrganisationRegistry.Organisation;

public class OutputRecord
{
    protected OutputRecord(string reference, Guid parentOrganisationId, string name, int sortOrder)
    {
        Reference = reference;
        ParentOrganisationId = parentOrganisationId;
        Name = name;
        SortOrder = sortOrder;
    }

    public string Reference { get; }
    public Guid ParentOrganisationId { get; }
    public string Name { get; }
    public DateOnly? Validity_Start { get; protected init; }
    public string? ShortName { get; protected init; }
    public Article? Article { get; protected init; }
    public DateOnly? OperationalValidity_Start { get; protected init; }
    public string? OvoNumber { get; private init; }
    public int SortOrder { get; }

    public static OutputRecord From(DeserializedRecord record,Guid parentOrganisationId, int sortOrder)
        => new(record.Reference.Value!, parentOrganisationId, record.Name.Value!, sortOrder)
        {
            Article = Article.Parse(record.Article.Value),
            ShortName = record.ShortName.Value,
            Validity_Start = record.Validity_Start.Value is { } validityStart
                ? DateOnly.ParseExact(validityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null,
            OperationalValidity_Start = record.Validity_Start.Value is { } operationalValidityStart
                ? DateOnly.ParseExact(operationalValidityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null
        };

    public OutputRecord WithOvoNumber(string ovoNumber)
        => new(Reference, ParentOrganisationId, Name, SortOrder)
        {
            Article = Article,
            ShortName = ShortName,
            Validity_Start = Validity_Start,
            OperationalValidity_Start = OperationalValidity_Start,
            OvoNumber = ovoNumber
        };
}
