namespace OrganisationRegistry
{
    using System;

    public struct ValidFrom: IEquatable<ValidFrom>, IComparable<ValidFrom>
    {
        public DateTime? DateTime { get; }

        public bool IsInfinite
            => !DateTime.HasValue;

        public ValidFrom(DateTime? dateTime)
            => DateTime = dateTime;

        public ValidFrom(int year, int month, int day)
            => DateTime = new DateTime(year, month, day);

        public static implicit operator DateTime?(ValidFrom validFrom)
            => validFrom.DateTime;

        public override string ToString()
            => DateTime.HasValue? DateTime.Value.ToString("yyyy-MM-dd") : "~";

        public bool IsInFutureOf(DateTime date, bool inclusive = false)
            => inclusive
                ? this >= new ValidFrom(date)
                : this > new ValidFrom(date);

        public bool IsInPastOf(DateTime date, bool inclusive = false)
            => inclusive
                ? this <= new ValidFrom(date)
                : this < new ValidFrom(date);

        public static bool operator ==(ValidFrom left, ValidFrom right)
            => left.Equals(right);

        public static bool operator !=(ValidFrom left, ValidFrom right)
            => !(left == right);

        public static bool operator <(ValidFrom left, ValidFrom right)
            => left.CompareTo(right) < 0;

        public static bool operator <=(ValidFrom left, ValidFrom right)
            => left.CompareTo(right) <= 0;

        public static bool operator >(ValidFrom left, ValidFrom right)
            => left.CompareTo(right) > 0;

        public static bool operator >=(ValidFrom left, ValidFrom right)
            => left.CompareTo(right) >= 0;

        public int CompareTo(ValidFrom other)
        {
            if (!DateTime.HasValue && !other.DateTime.HasValue)
                return 0;

            if (!DateTime.HasValue)
                return -1;

            if (!other.DateTime.HasValue)
                return 1;

            return DateTime.Value.CompareTo(other.DateTime.Value);
        }

        public override bool Equals(object? obj)
            => obj is ValidFrom && Equals((ValidFrom) obj);

        public bool Equals(ValidFrom other)
            => DateTime == other.DateTime;

        public override int GetHashCode()
            => DateTime.GetHashCode();
    }
}
