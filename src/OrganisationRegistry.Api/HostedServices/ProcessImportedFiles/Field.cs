namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

public class Field
{
    private Field(string columnName, string? value, bool shouldHaveValue)
    {
        ColumnName = columnName;
        Value = value;
        ShouldHaveValue = shouldHaveValue;
    }

    public string ColumnName { get; }
    public string? Value { get; }
    public bool ShouldHaveValue { get; }

    public bool HasValue
        => !string.IsNullOrEmpty(Value);

    public static Field NoValue(string columnName)
        => new(columnName,null, false);

    public void Deconstruct(out string? value, out bool shouldHaveValue)
    {
        value = Value;
        shouldHaveValue = ShouldHaveValue;
    }

    public static Field FromValue(string columnName, string? value)
        => new(columnName, value, true);
}
