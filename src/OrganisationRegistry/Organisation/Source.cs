namespace OrganisationRegistry.Organisation;

using System;

public class Source : IEquatable<Source>
{
    public static readonly Source Kbo = new("KBO");
    public static readonly Source Wegwijs = new("WEGWIJS");

    public static readonly Source[] All = { Kbo, Wegwijs };

    private readonly string? _value;

    public Source(string? value)
        => _value = value;

    public bool Equals(Source? other)
        => other is { } && other._value == _value;

    public override bool Equals(object? obj)
        => obj is Source type && Equals(type);

    public override int GetHashCode()
        => _value?.GetHashCode() ?? 0;

    public override string ToString()
        => _value ?? "";

    public static implicit operator string(Source instance)
        => instance.ToString();

    public static bool operator ==(Source left, Source right)
        => Equals(left, right);

    public static bool operator !=(Source left, Source right)
        => !Equals(left, right);
}
