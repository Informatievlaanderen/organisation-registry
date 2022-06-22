namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using System.Collections.Generic;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Organisation.Import;

public class OutputRecordBuilder
{
    private readonly OrganisationParentIdentifier _parentOrganisationid;
    private readonly int _sortOrder;
    private DeserializedRecord _deserializedRecord;
    private readonly Dictionary<string, (Guid id, string name)> _labelTypes = new();

    public OutputRecordBuilder(string reference, OrganisationParentIdentifier parentOrganisationid, string name, int sortOrder)
    {
        _parentOrganisationid = parentOrganisationid;
        _sortOrder = sortOrder;
        _deserializedRecord = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, reference),
            Name = Field.FromValue(ColumnNames.Name, name),
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

    public OutputRecordBuilder AddLabel(Guid labelTypeId, string labelTypeName, string labelValue)
    {
        _labelTypes.Add(labelTypeName, (id: labelTypeId, name: labelTypeName));
        _deserializedRecord = _deserializedRecord with { Labels = _deserializedRecord.Labels.Add(Field.FromValue($"label#{labelTypeName}", labelValue)) };
        return this;
    }

    public CreateOrganisationsFromImportCommandItem Build()
        => _deserializedRecord.ToCommandItem(_labelTypes, _parentOrganisationid, _sortOrder);

    public static implicit operator CreateOrganisationsFromImportCommandItem(OutputRecordBuilder builder)
        => builder.Build();
}
