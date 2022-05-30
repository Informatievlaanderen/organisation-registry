namespace OrganisationRegistry.Infrastructure.Infrastructure;

public class ValueItem
{
    public string Value { get; }

    public ValueItem(string value)
        => Value = value;

    public static implicit operator string(ValueItem value)
        => value.Value;
}