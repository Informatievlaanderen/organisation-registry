namespace OrganisationRegistry.Organisation;

using System;

public class OrganisationSource : IEquatable<OrganisationSource>
{
    // public static readonly OrganisationSource Kbo = new("KBO");
    // public static readonly OrganisationSource Wegwijs = new("WEGWIJS");
    public static readonly OrganisationSource CsvImport = new("CSVIMPORT");

    public static readonly OrganisationSource[] All = { CsvImport };

    private readonly string? _value;

    public OrganisationSource(string? value)
        => _value = value;

    public bool Equals(OrganisationSource? other)
        => other is { } && other._value == _value;

    public override bool Equals(object? obj)
        => obj is OrganisationSource type && Equals(type);

    public override int GetHashCode()
        => _value?.GetHashCode() ?? 0;

    public override string ToString()
        => _value ?? string.Empty;

    public static implicit operator string(OrganisationSource? instance)
        => instance?.ToString() ?? string.Empty;

    public static bool operator ==(OrganisationSource left, OrganisationSource right)
        => Equals(left, right);

    public static bool operator !=(OrganisationSource left, OrganisationSource right)
        => !Equals(left, right);
}
