namespace OrganisationRegistry.Organisation;

using System;

public class LocationSource : IEquatable<LocationSource>
{
    public static readonly LocationSource Kbo = new("KBO");
    public static readonly LocationSource Wegwijs = new("WEGWIJS");

    public static readonly LocationSource[] All = { Kbo, Wegwijs };

    private readonly string? _value;

    public LocationSource(string? value)
        => _value = value;

    public bool Equals(LocationSource? other)
        => other is { } && other._value == _value;

    public override bool Equals(object? obj)
        => obj is LocationSource type && Equals(type);

    public override int GetHashCode()
        => _value?.GetHashCode() ?? 0;

    public override string ToString()
        => _value ?? "";

    public static implicit operator string(LocationSource instance)
        => instance.ToString();

    public static bool operator ==(LocationSource left, LocationSource right)
        => Equals(left, right);

    public static bool operator !=(LocationSource left, LocationSource right)
        => !Equals(left, right);
}
