namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using System.Collections.Generic;
using Organisation.Import;

public class CreateOrganisationsFromImportCommandItemBuilder
{
    private readonly OrganisationParentIdentifier _parentOrganisationid;
    private readonly int _sortOrder;
    private readonly CreateOrganisationsDeserializedRecordBuilder _deserializedRecordBuilder;
    private readonly Dictionary<string, (Guid id, string name)> _labelTypes = new();

    public CreateOrganisationsFromImportCommandItemBuilder(string reference, OrganisationParentIdentifier parentOrganisationid, string name, int sortOrder)
    {
        _parentOrganisationid = parentOrganisationid;
        _sortOrder = sortOrder;
        _deserializedRecordBuilder = new CreateOrganisationsDeserializedRecordBuilder(reference, name);
    }

    public CreateOrganisationsFromImportCommandItemBuilder WithArticle(string value)
    {
        _deserializedRecordBuilder.WithArticle(value);
        return this;
    }

    public CreateOrganisationsFromImportCommandItemBuilder WithShortName(string value)
    {
        _deserializedRecordBuilder.WithShortName(value);
        return this;
    }

    public CreateOrganisationsFromImportCommandItemBuilder WithValidityStart(string value)
    {
        _deserializedRecordBuilder.WithValidityStart(value);
        return this;
    }

    public CreateOrganisationsFromImportCommandItemBuilder WithOperationalValidityStart(string value)
    {
        _deserializedRecordBuilder.WithOperationalValidityStart(value);
        return this;
    }

    public CreateOrganisationsFromImportCommandItemBuilder AddLabel(Guid labelTypeId, string labelTypeName, string labelValue)
    {
        _labelTypes.Add(labelTypeName, (id: labelTypeId, name: labelTypeName));
        _deserializedRecordBuilder.AddLabel(labelTypeName, labelValue);
        return this;
    }

    public CreateOrganisationsFromImportCommandItem Build()
        => _deserializedRecordBuilder.Build().ToCommandItem(_labelTypes, _parentOrganisationid, _sortOrder);

    public static implicit operator CreateOrganisationsFromImportCommandItem(CreateOrganisationsFromImportCommandItemBuilder builder)
        => builder.Build();
}
