namespace OrganisationRegistry;

using System;

public struct ValidTo: IEquatable<ValidTo>, IComparable<ValidTo>
{
    public DateTime? DateTime { get; }

    public bool IsInfinite
        => !DateTime.HasValue;

    public ValidTo(DateTime? dateTime)
        => DateTime = dateTime;

    public ValidTo(int year, int month, int day)
        => DateTime = new DateTime(year, month, day);

    public static implicit operator DateTime? (ValidTo validTo)
        => validTo.DateTime;

    public override string ToString()
        => DateTime.HasValue ? DateTime.Value.ToString("yyyy-MM-dd") : "~";

    public bool IsInFutureOf(DateTime date, bool inclusive = false)
        => inclusive
            ? this >= new ValidTo(date)
            : this > new ValidTo(date);

    public bool IsInPastOf(DateTime date, bool inclusive = false)
        => inclusive
            ? this <= new ValidTo(date)
            : this < new ValidTo(date);

    public static bool operator ==(ValidTo left, ValidTo right)
        => left.Equals(right);

    public static bool operator !=(ValidTo left, ValidTo right)
        => !(left == right);

    public static bool operator <(ValidTo left, ValidTo right)
        => left.CompareTo(right) < 0;

    public static bool operator <=(ValidTo left, ValidTo right)
        => left.CompareTo(right) <= 0;

    public static bool operator >(ValidTo left, ValidTo right)
        => left.CompareTo(right) > 0;

    public static bool operator >=(ValidTo left, ValidTo right)
        => left.CompareTo(right) >= 0;

    public int CompareTo(ValidTo other)
    {
        if (!DateTime.HasValue && !other.DateTime.HasValue)
            return 0;

        if (!DateTime.HasValue)
            return 1;

        if (!other.DateTime.HasValue)
            return -1;

        return DateTime.Value.CompareTo(other.DateTime.Value);
    }

    public override bool Equals(object? obj)
        => obj is ValidTo && Equals((ValidTo) obj);

    public bool Equals(ValidTo other)
        => DateTime == other.DateTime;

    public override int GetHashCode()
        => DateTime.GetHashCode();
}