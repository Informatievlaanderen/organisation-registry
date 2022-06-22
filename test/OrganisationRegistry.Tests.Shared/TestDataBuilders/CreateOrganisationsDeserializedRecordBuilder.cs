namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;

public class CreateOrganisationsDeserializedRecordBuilder
{
    private DeserializedRecord _deserializedRecord;

    public CreateOrganisationsDeserializedRecordBuilder(string reference, string name)
    {
        _deserializedRecord = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, reference),
            Name = Field.FromValue(ColumnNames.Name, name),
        };
    }

    public CreateOrganisationsDeserializedRecordBuilder WithArticle(string value)
    {
        _deserializedRecord = _deserializedRecord with { Article = Field.FromValue(ColumnNames.Article, value) };
        return this;
    }

    public CreateOrganisationsDeserializedRecordBuilder WithShortName(string value)
    {
        _deserializedRecord = _deserializedRecord with { ShortName = Field.FromValue(ColumnNames.ShortName, value) };
        return this;
    }

    public CreateOrganisationsDeserializedRecordBuilder WithValidityStart(string value)
    {
        _deserializedRecord = _deserializedRecord with { Validity_Start = Field.FromValue(ColumnNames.Validity_Start, value) };
        return this;
    }

    public CreateOrganisationsDeserializedRecordBuilder WithOperationalValidityStart(string value)
    {
        _deserializedRecord = _deserializedRecord with { OperationalValidity_Start = Field.FromValue(ColumnNames.OperationalValidity_Start, value) };
        return this;
    }

    public CreateOrganisationsDeserializedRecordBuilder AddLabel(string labelTypeName, string labelValue)
    {
        _deserializedRecord = _deserializedRecord with { Labels = _deserializedRecord.Labels.Add(Field.FromValue($"label#{labelTypeName}", labelValue)) };
        return this;
    }

    public CreateOrganisationsDeserializedRecordBuilder WithParent(string value)
    {
        _deserializedRecord = _deserializedRecord with { Parent = Field.FromValue(ColumnNames.Parent, value) };
        return this;
    }

    public DeserializedRecord Build()
        => _deserializedRecord;

    public static implicit operator DeserializedRecord(CreateOrganisationsDeserializedRecordBuilder builder)
        => builder.Build();
}
