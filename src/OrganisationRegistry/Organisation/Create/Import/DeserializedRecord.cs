namespace OrganisationRegistry.Organisation.Import;

public record DeserializedRecord
{
    public Field Reference { get; init; } = Field.NoValue(ColumnNames.Reference);
    public Field Parent { get; init; } = Field.NoValue(ColumnNames.Parent);
    public Field Name { get; init; } = Field.NoValue(ColumnNames.Name);
    public Field Validity_Start { get; init; } = Field.NoValue(ColumnNames.Validity_Start);
    public Field ShortName { get; init; } = Field.NoValue(ColumnNames.ShortName);
    public Field Article { get; init; } = Field.NoValue(ColumnNames.Article);
    public Field OperationalValidity_Start { get; init; } = Field.NoValue(ColumnNames.OperationalValidity_Start);
}
