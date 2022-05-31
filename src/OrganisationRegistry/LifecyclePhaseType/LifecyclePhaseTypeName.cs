namespace OrganisationRegistry.LifecyclePhaseType;

using System;

public class LifecyclePhaseTypeName : IEquatable<LifecyclePhaseTypeName>
{
    private string Value { get; }

    public LifecyclePhaseTypeName(string value) => Value = value;

    public static implicit operator string(LifecyclePhaseTypeName cast) => cast.Value;

    public override bool Equals(object? obj) => obj is LifecyclePhaseTypeName && Equals((LifecyclePhaseTypeName) obj);
    public bool Equals(LifecyclePhaseTypeName? other) => other is { } && Value == other.Value;
    public override int GetHashCode() => Value.GetHashCode();
}
