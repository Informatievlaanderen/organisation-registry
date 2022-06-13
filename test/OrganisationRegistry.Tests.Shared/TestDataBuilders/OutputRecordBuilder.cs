namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using Organisation.Import;

public class OutputRecordBuilder
{
    private readonly OrganisationParentIdentifier _parentOrganisationid;
    private readonly int _sortOrder;
    private DeserializedRecord _deserializedRecord;

    public OutputRecordBuilder(string reference, OrganisationParentIdentifier parentOrganisationid, string name, int sortOrder)
    {
        _parentOrganisationid = parentOrganisationid;
        _sortOrder = sortOrder;
        _deserializedRecord = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, reference),
            Name = Field.FromValue(ColumnNames.Name, name)
        };
    }

    public OutputRecordBuilder WithArticle(string value)
    {
        _deserializedRecord = _deserializedRecord with { Article = Field.FromValue(ColumnNames.Article, value) };
        return this;
    }

    public OutputRecordBuilder WithShortName(string value)
    {
        _deserializedRecord = _deserializedRecord with { ShortName = Field.FromValue(ColumnNames.ShortName, value) };
        return this;
    }

    public OutputRecordBuilder WithValidityStart(string value)
    {
        _deserializedRecord = _deserializedRecord with { Validity_Start = Field.FromValue(ColumnNames.Validity_Start, value) };
        return this;
    }

    public OutputRecordBuilder WithOperationalValidityStart(string value)
    {
        _deserializedRecord = _deserializedRecord with { OperationalValidity_Start = Field.FromValue(ColumnNames.OperationalValidity_Start, value) };
        return this;
    }

    public OutputRecord Build()
        => OutputRecord.From(_deserializedRecord, _parentOrganisationid, _sortOrder);

    public static implicit operator OutputRecord(OutputRecordBuilder builder)
        => builder.Build();
}
