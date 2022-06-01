namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Globalization;
using Organisation;

public class OutputRecord
{
    private OutputRecord(string reference, string name)
    {
        Reference = reference;
        Name = name;
    }

    public string Reference { get; }
    public string? Parent { get; private init; }
    public string Name { get; }
    public DateOnly? Validity_Start { get; private init; }
    public string? ShortName { get; private init; }
    public Article? Article { get; private init; }
    public DateOnly? OperationalValidity_Start { get; private init; }

    public static OutputRecord From(DeserializedRecord record)
        => new(record.Reference.Value!, record.Name.Value!)
        {
            Parent = record.Parent.Value,
            Article = Article.Parse(record.Article.Value),
            ShortName = record.ShortName.Value,
            Validity_Start = record.Validity_Start.Value is { } validityStart
                ? DateOnly.ParseExact(validityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null,
            OperationalValidity_Start = record.Validity_Start.Value is { } operationalValidityStart
                ? DateOnly.ParseExact(operationalValidityStart, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                : null
        };
}
